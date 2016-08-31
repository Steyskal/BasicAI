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
	[TaskDescription("Seeks an existing target using given scanners." +
	                 "\n@return Success, if the target is in scan range." +
	                 "\n@return Failure, if target is no longer in scan range.")]
	public class Seek : Conditional
	{	
		public SharedTransform Target;

		private FieldOfView _fieldOfView;

		public override void OnStart()
		{
			_fieldOfView = Owner.GetComponentInChildren<FieldOfView> ();
		}

		public override TaskStatus OnUpdate()
		{
			if (Target.Value != null)
			{
				if (_fieldOfView.SeekVisibleTarget(Target.Value))
				{
					return TaskStatus.Success;
				}
			}
			else if(Target.Value == null)
			{
				Debug.LogWarning("There is no target set, or scanned to be seeked.");
			}
			
			return TaskStatus.Failure;
		}
	}
}
