using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class AIController : MonoBehaviour
{
	public ExternalBehavior StandardBehavior;
	public BehaviorTree CurrentBehaviorTree;
	public Transform ScannersParent;
	public Transform WaypointsParent;
	
	[Header("Read-Only")]
	[SerializeField]
	private int _behaviorPriority = 0;
	[SerializeField]
	private List<BehaviorSetup> RegisteredBehaviors = new List<BehaviorSetup>();

	private BehaviorSetup _activeBehaviorSetup;

	void Start()
	{
		OnActivationChangedListener (true);

		GetComponents<BehaviorSetup>(RegisteredBehaviors);

		for (int i = 0; i < RegisteredBehaviors.Count; i++)
		{
			RegisteredBehaviors[i].OnConditionMetEvent.AddListener(OnBehaviorConditionMetListener);
		}
	}

	private void OnActivationChangedListener(bool isActive)
	{
		_behaviorPriority = 0;

		if(StandardBehavior)
		{
			CurrentBehaviorTree.ExternalBehavior = StandardBehavior;
			CurrentBehaviorTree.enabled = isActive;
		}

		if(ScannersParent)
			CurrentBehaviorTree.SetVariableValue("ScannersParent", ScannersParent);

		if(WaypointsParent)
			CurrentBehaviorTree.SetVariableValue("WaypointsParent", WaypointsParent);

		if(StandardBehavior && isActive)
			CurrentBehaviorTree.EnableBehavior();
	}

	private void OnBehaviorConditionMetListener(BehaviorSetup behaviorSetup)
	{
		if(behaviorSetup.Priority >= _behaviorPriority)
		{
			if(_activeBehaviorSetup)
				_activeBehaviorSetup.OnBehaviourEndedEvent.RemoveListener(OnBehaviorEndedListener);

			_activeBehaviorSetup = behaviorSetup;
			_behaviorPriority = _activeBehaviorSetup.Priority;

			CurrentBehaviorTree.ExternalBehavior = _activeBehaviorSetup.Behavior;

			if(_activeBehaviorSetup.WillUseScanners)
				CurrentBehaviorTree.SetVariableValue("ScannersParent", ScannersParent);

			if(_activeBehaviorSetup.WillUseSetupTarget)
				CurrentBehaviorTree.SetVariableValue("SetupTarget", _activeBehaviorSetup.FoundTarget);

			if(_activeBehaviorSetup.WillUseWaypoints)
				CurrentBehaviorTree.SetVariableValue("WaypointsParent", WaypointsParent);

			_activeBehaviorSetup.OnBehaviourEndedEvent.AddListener(OnBehaviorEndedListener);

			CurrentBehaviorTree.EnableBehavior();
		}
	}

	private void OnBehaviorEndedListener()
	{
		if(CurrentBehaviorTree.enabled)
			OnActivationChangedListener(true);
	}
}