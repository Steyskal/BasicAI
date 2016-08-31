using UnityEngine;
using System.Collections;

public class OnAllyAlertBehaviorSetup : BehaviorSetup
{
	private bool _isAlerted = false;
	public bool IsAlerted
	{
		get
		{
			return _isAlerted;
		}

		set
		{
			_isAlerted = value;

			if(_isAlerted)
				Invoke("StopAlert", Duration);
		}
	}

	public void Alert(Transform foundTarget)
	{
		if(!IsAlerted)
		{
			IsAlerted = true;
			ShouldCheckForTarget = true;

			if(foundTarget)
			{
				FoundTarget = foundTarget;
				ShouldCheckForTarget = false;
			}

			InvokeSetup();
		}
	}

	private void StopAlert()
	{
		IsAlerted = false;
	}
}
