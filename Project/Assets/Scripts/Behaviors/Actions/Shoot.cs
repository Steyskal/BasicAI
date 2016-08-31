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
	public class Shoot : Action
	{
		private Shooter _shooter;

		public override void OnAwake ()
		{
			_shooter = Owner.GetComponentInParent<Shooter> ();
		}

		public override TaskStatus OnUpdate ()
		{
			_shooter.Shoot ();

			return TaskStatus.Success;
		}
	}
}
