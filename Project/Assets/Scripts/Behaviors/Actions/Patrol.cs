using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

#if !(UNITY_4_3 || UNITY_4_4)
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
#endif

[TaskCategory ("CustomAI/Movement")]
[TaskDescription ("Patrols the agent around given waypoints." +
"\n@return Success: if the loop is unchecked and the agent has finished its route." +
"\n@return Running: if the agent is patrolling.")]
public class Patrol : Action
{
	[@Tooltip ("The movement speed of the agent.")]
	public SharedFloat
		MovementSpeed;
	[@Tooltip ("Deviation allowed when following route waypoints (0 means no deviation).")]
	public float
		AllowedRouteDeviation;
	[@Tooltip ("Time the agent will wait when it reaches its next waypoint.")]
	public float
		WaitAtWaypointTime;
	[@Tooltip ("Should the agent loop this action.")]
	public bool
		Loop;
	[@Tooltip ("The parent of movement waypoints.")] 
	public SharedTransform
		WaypointsParent;
	[@Tooltip ("The current position the agent is traveling to.")] 
	public SharedVector3
		CurrentWaypointPosition;

	private List<Transform> _waypoints = new List<Transform> ();
	private int _waypointIndex;
	private bool _isMoving;
	private float _moveTime;

	private Transform _transform;
	private Rigidbody2D _rigidbody2D;

	public override void OnAwake ()
	{
		_transform = transform;
		_rigidbody2D = Owner.GetComponentInParent<Rigidbody2D> ();

		for (int i = 0; i < WaypointsParent.Value.childCount; i++)
		{
			_waypoints.Add (WaypointsParent.Value.GetChild (i));
		}

		_isMoving = true;

		FindTheClosestWaypoint ();
	}

	public override TaskStatus OnUpdate ()
	{
		if (Time.time >= _moveTime)
		{
			Move ();

			if (!_isMoving)
				return TaskStatus.Success;
		}

		return TaskStatus.Running;
	}

	private void FindTheClosestWaypoint ()
	{
		float distance = Mathf.Infinity;
		float localDistance;

		for (int i = 0; i < _waypoints.Count; i++)
		{
			localDistance = Vector3.Magnitude (_transform.position - _waypoints [i].position);

			if (localDistance < distance)
			{
				distance = localDistance;
				_waypointIndex = i;
			}
		}
	}

	private void MoveToPosition (Vector3 position)
	{
		Vector2 movementDirection = (Vector2)position - _rigidbody2D.position;
		movementDirection.Normalize ();

		_rigidbody2D.MovePosition (_rigidbody2D.position + movementDirection * MovementSpeed.Value * Time.deltaTime);
	}

	private void Move ()
	{
		if ((_waypoints.Count != 0) && (_isMoving))
		{
			CurrentWaypointPosition.Value = _waypoints [_waypointIndex].position;
			MoveToPosition (CurrentWaypointPosition.Value);

			if (Vector3.Distance (CurrentWaypointPosition.Value, _rigidbody2D.position) <= AllowedRouteDeviation)
			{
				_waypointIndex++;
				_moveTime = Time.time + WaitAtWaypointTime;
			}

			if (_waypointIndex >= _waypoints.Count)
			{
				if (Loop)
					_waypointIndex = 0;
				else
					_isMoving = false;
			}
		}
	}
}
