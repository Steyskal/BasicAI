using UnityEngine;
using System.Collections;

public class OnCollisionEnterBehaviorSetup : BehaviorSetup
{
	void OnCollisionEnter2D(Collision2D other)
	{
		InvokeSetup();
	}
}
