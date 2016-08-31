using UnityEngine;
using System.Collections;

using Happy;

public class DisplayWaypointRoute : MonoBehaviour
{
	void OnDrawGizmos()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GizmosExtension.DrawLine (transform.GetChild (i).position, transform.GetChild ((i + 1) % transform.childCount).position, Color.cyan);
		}
	}
}
