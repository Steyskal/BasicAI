using UnityEngine.Events;

namespace Happy
{
	[System.Serializable]
	public class CustomUnityEvent : UnityEvent
	{
	}

	[System.Serializable]
	public class CustomUnityEvent<T0> : UnityEvent<T0>
	{
	}

	[System.Serializable]
	public class CustomUnityEvent<T0, T1> : UnityEvent<T0, T1>
	{
	}

	[System.Serializable]
	public class CustomUnityEvent<T0, T1, T2> : UnityEvent<T0, T1, T2>
	{
	}

	[System.Serializable]
	public class CustomUnityEvent<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3>
	{
	}
}
