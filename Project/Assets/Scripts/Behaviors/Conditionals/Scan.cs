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
	[TaskCategory("CustomAI/Scanners")]
	[TaskDescription("Scans for a target using given scanners." +
	                 "\n@return Success, if a target is found (sets Target to traget found)." +
	                 "\n@return Failure, if no targets are found.")]
	public class Scan : Conditional
	{
		public SharedTransform Target;

		private FieldOfView _fieldOfView;

		public override void OnStart()
		{
			Target.Value = null;

			_fieldOfView = Owner.GetComponentInChildren<FieldOfView> ();
		}

		public override TaskStatus OnUpdate()
		{
			if (!Target.Value)
			{
				Target.Value = _fieldOfView.FindVisibleTarget();
			}
			else
			{
				if (!_fieldOfView.SeekVisibleTarget(Target.Value))
				{
					Target.Value = null;
				}
			}

			if (Target.Value)
				return TaskStatus.Success;

			return TaskStatus.Failure;
		}
	}
}
