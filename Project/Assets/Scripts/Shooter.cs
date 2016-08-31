using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shooter : MonoBehaviour
{
	public GameObject ProjectilePrefab;
	public float ProjectileSpeed = 10.0f;
	public Transform ProjectileParent;
	public List<Transform> FiringPoints;

	public void Shoot()
	{
		for (int i = 0; i < FiringPoints.Count; i++)
		{
			GameObject projectileClone = (GameObject) Instantiate (ProjectilePrefab, FiringPoints [i].position, Quaternion.identity);
			projectileClone.transform.SetParent (ProjectileParent);

			Vector2 direction = FiringPoints [i].right;
			projectileClone.GetComponent<Rigidbody2D> ().velocity = direction * ProjectileSpeed;
		}
	}
}
