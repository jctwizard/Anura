using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform frog1;
    public Transform frog2;
    public float rate = 1;
    private Vector3 midPoint;

	void Update () {
        midPoint = Vector3.Lerp(frog1.position, frog2.position, 0.5f);
        transform.position = Vector3.Lerp(transform.position, new Vector3(midPoint.x, midPoint.y, transform.position.z), Time.deltaTime * rate);
    }
}
