using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour
{
	public float DestructionTimer = 2.0f;

	void Awake()
	{
		Invoke ("TimedDestroy", DestructionTimer);
	}

	private void TimedDestroy()
	{
		Destroy (gameObject);
	}
}
