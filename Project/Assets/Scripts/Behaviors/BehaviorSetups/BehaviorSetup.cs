using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

using Happy;

public abstract class BehaviorSetup : MonoBehaviour
{
	[Header("Behavior Setup Properties")]
	public int Priority = 0;
	public ExternalBehavior Behavior;
	public int Duration = 5;
	public bool WillUseScanners = false;
	public bool WillUseWaypoints = false;
	public bool WillUseSetupTarget = false;

	[Header("Communication Properties")]
	public float Radius = 15.0f;
	public bool ShouldCheckForTarget = false;
	public LayerMask TargetLayerMask;
	public bool ShouldAlertAllies = false;
	public LayerMask AlliesLayerMask;

	[Header("Read-Only")]
	[SerializeField]
	private int _timer;
	public Transform FoundTarget;
	
	public CustomUnityEvent<BehaviorSetup> OnConditionMetEvent = new CustomUnityEvent<BehaviorSetup>();
	public CustomUnityEvent OnBehaviourEndedEvent = new CustomUnityEvent();

	private Transform _transform;

	void Awake()
	{
		_transform = transform;
	}

	protected void InvokeSetup()
	{
		// Logical implication
		if(!ShouldCheckForTarget || IsTargetFound())
		{
			if(ShouldAlertAllies)
				AlertAllies();

			OnConditionMetEvent.Invoke(this);
			
			_timer = Duration;
			StartCoroutine("BehaviorTick");
		}
	}

	protected virtual void InvokeEnd()
	{
		OnBehaviourEndedEvent.Invoke();
	}

	public Transform CheckForTarget()
	{
		Collider2D targetCollider2D = Physics2D.OverlapCircle(_transform.position, Radius, TargetLayerMask.value);

		if(targetCollider2D)
			return targetCollider2D.transform;

		return null;
	}

	private bool IsTargetFound()
	{
		FoundTarget = CheckForTarget();

		return FoundTarget;
	}

	private void AlertAllies()
	{
		AlertAllies (FoundTarget);
	}

	public void AlertAllies(Transform target)
	{
		Collider2D myCollider2D = _transform.GetComponentInParent<Collider2D>();

		Collider2D[] possibleAllies = Physics2D.OverlapCircleAll(_transform.position, Radius, AlliesLayerMask.value);

		for (int i = 0; i < possibleAllies.Length; i++)
		{
			OnAllyAlertBehaviorSetup allyBehaviorSetup = possibleAllies[i].GetComponent<OnAllyAlertBehaviorSetup>();

			if(allyBehaviorSetup)
			{
				if(possibleAllies[i].Equals(myCollider2D))
					allyBehaviorSetup.IsAlerted = true;
				else
					allyBehaviorSetup.Alert(target);
			}		
		}
	}

	private IEnumerator BehaviorTick()
	{
		while(_timer > 0)
		{
			yield return new WaitForSeconds(1.0f);

			_timer--;
		}

		InvokeEnd();
	}

	void OnDrawGizmos()
	{
		GizmosExtension.DrawCircle(transform.position, Radius, Color.white);
	}
}
