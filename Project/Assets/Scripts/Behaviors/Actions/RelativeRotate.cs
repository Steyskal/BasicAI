using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

#if !(UNITY_4_3 || UNITY_4_4)
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
#endif

namespace BehaviorDesigner.LastEncounter
{
	[TaskCategory("CustomAI/Movement")]
	[TaskDescription("Rotates relative to the target." +
	                 "\n@return Running: if the agent is rotating relative to the target." +
	                 "\n@return Failure: if there is no target set")]
	public class RelativeRotate : Action
	{
		public enum RotationType
		{
			ToTarget,
			FromTarget
		}

		public RotationType Rotation = RotationType.ToTarget;
		[@Tooltip("The rotation speed of the agent.")]
		public SharedFloat
			RotationSpeed;
		[@Tooltip("The target that the agent will rotate relative to.")]
		public SharedTransform
			Target;
		[@Tooltip("The position that the agent will rotate relative to if there is no Target set.")]
		public SharedVector3
			Position;

		private Transform _transform;
		private Rigidbody2D _rigidbody2D;

		private Vector3 _position
		{
			get
			{
				return Target.Value ? Target.Value.position : Position.Value;
			}
		}

		public override void OnAwake()
		{
			_transform = Owner.transform.parent;
			_rigidbody2D = Owner.GetComponentInParent<Rigidbody2D>();

		}

		public override TaskStatus OnUpdate ()
		{
			Rotate ();
			return TaskStatus.Running;
		}

		private void Rotate()
		{
			Vector3 direction = Vector3.zero;

			switch (Rotation)
			{
				case RotationType.ToTarget:
					direction = _position - _transform.position;
					break;

				case RotationType.FromTarget:
					direction = _transform.position - _position;
					break;
			}

			float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			q = Quaternion.Slerp (_transform.rotation, q, Time.deltaTime * RotationSpeed.Value);
			_rigidbody2D.MoveRotation (q.eulerAngles.z);
		}
	}
}
