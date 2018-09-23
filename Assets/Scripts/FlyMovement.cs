using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMovement : MonoBehaviour
{
	public float rotateSpeed = 10.0f;
	public float moveSpeed = 1.0f;
	public float minTimer = 0.5f;
	public float maxTimer = 6.0f;

	private bool rotatingClockwise = true;
	private float timer = 0;

	void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			timer = Random.Range(minTimer, maxTimer);
			rotatingClockwise = !rotatingClockwise;
		}

		if (rotatingClockwise)
		{
			transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
		}
		else
		{
			transform.Rotate(-Vector3.forward * rotateSpeed * Time.deltaTime);
		}

		transform.position += transform.right * moveSpeed * Time.deltaTime;   
    }
}
