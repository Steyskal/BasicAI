using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public float MovementSpeed = 5.0f;

	private float _moveH;
	private float _moveV;

	private Rigidbody2D _rigidbody2D;

	private Shooter _shooter;

	void Awake ()
	{
		_rigidbody2D = GetComponent<Rigidbody2D> ();
		_shooter = GetComponent<Shooter> ();
	}

	void Update ()
	{
		_moveH = Input.GetAxisRaw ("Horizontal");
		_moveV = Input.GetAxisRaw ("Vertical");

		if (Input.GetMouseButtonDown (0))
			_shooter.Shoot ();
	}

	void FixedUpdate()
	{
		RotateToMousePosition ();
		Move ();
	}

	private void RotateToMousePosition()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		Vector3 direction = mousePosition - (Vector3) _rigidbody2D.position;
		float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;

		_rigidbody2D.rotation = angle;
	}

	private void Move()
	{
		Vector2 movementDirection = new Vector2 (_moveH, _moveV);
		movementDirection.Normalize ();
		_rigidbody2D.MovePosition (_rigidbody2D.position + movementDirection * MovementSpeed * Time.deltaTime);
	}
}
