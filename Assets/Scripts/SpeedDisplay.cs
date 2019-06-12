using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDisplay : MonoBehaviour {

	// Use this for initialization
	public WheelCollider flWheelCollider;
	public float currentSpeed;
	private UILabel label;
	public Transform pointContainer;
	private float zRotation;

	void Start () {
		label = this.GetComponent<UILabel>();
		zRotation = pointContainer.eulerAngles.z;

	}
	
	// Update is called once per frame
	void Update () {
		
		currentSpeed = flWheelCollider.rpm  * (flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;
		currentSpeed = Mathf.Round(currentSpeed);
		label.text = currentSpeed.ToString();

		float newZRotation = zRotation - currentSpeed * (270/140f) ;
		pointContainer.eulerAngles = new Vector3(0, 0, newZRotation);

		
	}
}
