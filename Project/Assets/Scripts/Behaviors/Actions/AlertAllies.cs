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
	public class AlertAllies : Action
	{
		public override TaskStatus OnUpdate ()
		{
			BehaviorSetup behaviorSetup = Owner.GetComponent<BehaviorSetup> ();
			behaviorSetup.AlertAllies (behaviorSetup.CheckForTarget());

			return TaskStatus.Success;
		}
	}
}
