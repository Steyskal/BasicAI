using UnityEngine;
using System.Collections;

namespace Happy
{
	/// <summary>
	///	The Singleton class will make any class that inherits from it a singleton automatically.
	/// </summary>
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		protected static T _Instance;

		public static T Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = (T)FindObjectOfType (typeof(T));

					if (_Instance == null)
					{
						Debug.LogError ("An instance of " + typeof(T) +
						" is needed in the scene, but there is none!");
					}
				}

				return _Instance;
			}
		}
	}
}