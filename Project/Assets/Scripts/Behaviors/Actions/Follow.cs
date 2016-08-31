using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

#if !(UNITY_4_3 || UNITY_4_4)
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
#endif

[TaskCategory ("CustomAI/Movement")]
[TaskDescription ("Follows the target." +
"\n@return Running: if the agent is following the target." +
"\n@return Failure: if there is no target set or scanned to be followed.")]
public class Follow : Action
{
	[@Tooltip ("The movement speed of the agent.")]
	public SharedFloat
	MovementSpeed;
	[@Tooltip ("The distance the agent will keep while following.")]
	public float
		DistanceToKeep;
	[@Tooltip ("The target that the agent is following.")]
	public SharedTransform
		Target;

	private Vector3 _offsetTarget;

	private Transform _transform;
	private Rigidbody2D _rigidbody2D;

	public override void OnAwake ()
	{
		_transform = transform;
		_rigidbody2D = Owner.GetComponentInParent<Rigidbody2D> ();
	}

	public override TaskStatus OnUpdate ()
	{
		if (Target.Value != null)
			Move ();
		else
		{
			Debug.LogWarning ("There is no target set, or scanned to be followed.");
			return TaskStatus.Failure;
		}

		return TaskStatus.Running;
	}

	private void MoveToPosition (Vector3 position)
	{
		Vector2 movementDirection = (Vector2)position - _rigidbody2D.position;
		movementDirection.Normalize ();

		_rigidbody2D.MovePosition (_rigidbody2D.position + movementDirection * MovementSpeed.Value * Time.deltaTime);
	}

	private void Move ()
	{
		if (Vector3.Distance (Target.Value.position, _transform.position) > DistanceToKeep)
		{
			_offsetTarget = Target.Value.position - (DistanceToKeep * (Target.Value.position - _transform.position).normalized);
			MoveToPosition (_offsetTarget);
		}
		else
		{
			_rigidbody2D.velocity = Vector2.zero;
			_rigidbody2D.angularVelocity = 0.0f;
		}
	}
}
