using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

#if !(UNITY_4_3 || UNITY_4_4)
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
#endif


	[TaskCategory("CustomAI")]
	[TaskDescription("Performs distance comparison between a set of waypoints(using the closest waypoint) and an agent againts given critical value: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
	public class DistanceComparison : Conditional
	{
		public enum OperationType
		{
			LessThan,
			LessThanOrEqualTo,
			EqualTo,
			NotEqualTo,
			GreaterThanOrEqualTo,
			GreaterThan
		}

		[@Tooltip("[ReadOnly]" +
			"\nCurrent distance.")]
		[SerializeField]
		private float
		_distance;

		[@Tooltip("The operation to perform")]
		public OperationType
		Operation;
		[@Tooltip("The value which will be used for comparison.")]
		public SharedFloat
		DistanceCriticalValue;
		[@Tooltip("The parent of movement waypoints.")] 
		public SharedTransform
		WaypointsParent;
		private List<Vector3> _waypoints = new List<Vector3> ();

		public override void OnAwake ()
		{
			if (WaypointsParent.Value.childCount != 0)
			{
				for (int i = 0; i < WaypointsParent.Value.childCount; i++)
				{
					_waypoints.Add (WaypointsParent.Value.GetChild (i).position);
				}
			}else
			{
				//Debug.LogWarning("The waypoint parent has no children, using his transform as distance reference!");
				_waypoints.Add(WaypointsParent.Value.position);
			}

		}

		public override TaskStatus OnUpdate ()
		{
			FindCurrentDistance ();

			switch (Operation) {
				case OperationType.LessThan:
					return _distance < DistanceCriticalValue.Value ? TaskStatus.Success : TaskStatus.Failure;
				case OperationType.LessThanOrEqualTo:
					return _distance <= DistanceCriticalValue.Value ? TaskStatus.Success : TaskStatus.Failure;
				case OperationType.EqualTo:
					return _distance == DistanceCriticalValue.Value ? TaskStatus.Success : TaskStatus.Failure;
				case OperationType.NotEqualTo:
					return _distance != DistanceCriticalValue.Value ? TaskStatus.Success : TaskStatus.Failure;
				case OperationType.GreaterThanOrEqualTo:
					return _distance >= DistanceCriticalValue.Value ? TaskStatus.Success : TaskStatus.Failure;
				case OperationType.GreaterThan:
					return _distance > DistanceCriticalValue.Value ? TaskStatus.Success : TaskStatus.Failure;
			}

			return TaskStatus.Failure;
		}

		private void FindCurrentDistance ()
		{
			float distance = Mathf.Infinity;
			float localDistance;

			for (int i = 0; i < _waypoints.Count; i++)
			{
				localDistance = Vector3.Magnitude (Owner.transform.position - _waypoints [i]);

				if (localDistance < distance)
				{
					distance = localDistance;
				}
			}

			_distance = distance;
		}

		public override void OnReset ()
		{
			Operation = OperationType.LessThan;
			WaypointsParent.Value = null;
		}
	}
