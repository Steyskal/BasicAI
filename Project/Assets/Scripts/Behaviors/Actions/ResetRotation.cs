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
	[TaskCategory("CustomAI")]
	[TaskDescription("Resets the agent to its original rotation." +
	                 "\n@return Succes: if the agent is reset to its original rotation.")]
	public class ResetRotation : Action
	{
		public SharedFloat
			RotationSpeed;

		private Transform _transform;
		private Rigidbody2D _rigidbody2D;

		private bool _isRotationSet = false;
		private Quaternion _originalRotation;

		public override void OnAwake()
		{
			_transform = transform.parent;
			_rigidbody2D = Owner.GetComponentInParent<Rigidbody2D>();

			if(!_isRotationSet)
			{
				_originalRotation = _transform.parent.rotation;
				_isRotationSet = true;
			}
		}

		public override TaskStatus OnUpdate ()
		{	
			if(_transform.rotation != _originalRotation)
			{
				Quaternion q = Quaternion.RotateTowards(_transform.rotation, _originalRotation, Time.deltaTime * RotationSpeed.Value);
				_rigidbody2D.MoveRotation(q.eulerAngles.z);

				return TaskStatus.Running;
			}

			return TaskStatus.Success;
		}
	}
}