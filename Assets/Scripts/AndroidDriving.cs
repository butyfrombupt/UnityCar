using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidDriving : MonoBehaviour {

	// Use this for initialization
	public Transform centerOfMass;
	[HideInInspector]
	public WheelCollider flWheelCollider;
	[HideInInspector]
	public WheelCollider frWheelCollider;
	[HideInInspector]
	public WheelCollider rlWheelCollider;
	[HideInInspector]
	public WheelCollider rrWheelCollider;

	public Transform flWheelModel;

	public Transform frWheelModel;

	public Transform rlWheelModel;

	public Transform rrWheelModel;


	public Transform flDiscBrake;

	public Transform frDiscBrake;

	public Transform rlDiscBrake;

	public Transform rrDiscBrake;

	public float motorTorque = 100;
	public float steerAngle = 10;
	public float maxSpeed = 140;
	public float minSpeed = 30;

	public float brakeTorque = 3500;
	private float currentSpeed;
	private bool isBreaking = false;


	public int[] speedArray;

	public AudioSource carEngineAudio;

	public ParticleEmitter leftEmitter;
	public ParticleEmitter rightEmitter;

	public AudioSource skidAudio;

	public GameObject leftLight;

	public GameObject rightLight;

	public GameObject skidMark;

	public AudioSource crashSound;

	void Awake() {
		InitProperty();
	}

	void Start () {
		
		this.GetComponent<Rigidbody>().centerOfMass  = centerOfMass.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		currentSpeed = flWheelCollider.rpm * (flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;
		if ((currentSpeed > 0 && Input.GetAxis("Vertical") < 0) || (currentSpeed < 0 && Input.GetAxis("Vertical") > 0)) {
			isBreaking = true;
		} else {
			isBreaking = false;
		}

		if ((currentSpeed > maxSpeed && Input.GetAxis("Vertical") > 0) || (currentSpeed < -minSpeed && Input.GetAxis("Vertical") < 0)) {
			flWheelCollider.motorTorque = 0;
			frWheelCollider.motorTorque = 0;
		} else {

			flWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
			frWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorTorque;
		}

		if (isBreaking) {
			flWheelCollider.motorTorque = 0;
			frWheelCollider.motorTorque = 0;

			flWheelCollider.brakeTorque = brakeTorque;
			frWheelCollider.brakeTorque = brakeTorque;
		} else {
			flWheelCollider.brakeTorque = 0;
			frWheelCollider.brakeTorque = 0;
		}

		flWheelCollider.steerAngle = Input.GetAxis("Horizontal") * steerAngle;
		frWheelCollider.steerAngle = Input.GetAxis("Horizontal") * steerAngle;

		RotateWheel();
		SteerWheel();
		EngineSound();
		ControlLight();
		Skid();
	}
	void RotateWheel() {
		flDiscBrake.Rotate(flWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);
		frDiscBrake.Rotate(frWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);

		rlDiscBrake.Rotate(rlWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);
		rrDiscBrake.Rotate(rrWheelCollider.rpm * 6 * Time.deltaTime * Vector3.right);

	}

	void SteerWheel(){
		Vector3 localEulerAngles = flWheelModel.localEulerAngles;
		localEulerAngles.y = flWheelCollider.steerAngle;

		flWheelModel.localEulerAngles = localEulerAngles;
		frWheelModel.localEulerAngles = localEulerAngles;
	} 
	void EngineSound() {
		int index = 0;

		for (int i = 0; i < speedArray.Length - 2; i++) {
			if (currentSpeed >= speedArray[i]) {
				index = i;
			}
		}
		int minSpeed = speedArray[index];
		int maxSpeed = speedArray[index + 1];

		carEngineAudio.pitch = 0.1f + (currentSpeed - minSpeed) / (maxSpeed - minSpeed) * 0.8f;
	}
	void ControlLight() {
		if (currentSpeed < -3) {
			leftLight.SetActive(true);
			rightLight.SetActive(true);
		} else {
			leftLight.SetActive(false);
			rightLight.SetActive(false);
		}
	}
	private Vector3 lastFLSkidPos = Vector3.zero;
	private Vector3 lastFRSkidPos = Vector3.zero;
	void Skid() {
		if (currentSpeed > 40 && Mathf.Abs(flWheelCollider.steerAngle) > 5) {
			bool isGround = false;
			WheelHit hit;
			if (flWheelCollider.GetGroundHit(out hit)) {
				isGround = true;
				leftEmitter.emit = true;

				if (lastFLSkidPos.x != 0 && lastFLSkidPos.y != 0 && lastFLSkidPos.z != 0) {
					Vector3 pos = hit.point;
					pos.y += 0.05f;
					Quaternion rotation = Quaternion.LookRotation( hit.point-lastFLSkidPos );
					GameObject.Instantiate(skidMark, pos, rotation);
				}

				lastFLSkidPos=hit.point;
			} else {
				lastFLSkidPos=Vector3.zero;
				leftEmitter.emit = false;
			}
			if (frWheelCollider.GetGroundHit(out hit)) {
				isGround = true;
				rightEmitter.emit = true;

				if (lastFRSkidPos.x != 0 && lastFRSkidPos.y != 0 && lastFRSkidPos.z != 0) {
					Vector3 pos = hit.point;
					pos.y += 0.05f;
					Quaternion rotation = Quaternion.LookRotation(hit.point - lastFRSkidPos);
					GameObject.Instantiate(skidMark, pos, rotation);
				}

				lastFRSkidPos = hit.point;

			} else {
				lastFRSkidPos = Vector3.zero;
				rightEmitter.emit = false;
			}

			if (skidAudio.isPlaying == false && isGround == true) {
				skidAudio.Play();
			} else if(skidAudio.isPlaying && isGround==false ) {
				skidAudio.Stop();
			}
		} else {
			if (skidAudio.isPlaying) {
				skidAudio.Stop();
			}
			leftEmitter.emit = false;
			rightEmitter.emit = false;
		}
	}
	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Wall") {
			crashSound.Play();
		}
	}
	void InitProperty() {
		this.flWheelCollider = transform.Find("WheelFL/WheelFlCollider").GetComponent<WheelCollider>();
		this.frWheelCollider = transform.Find("WheelFR/WheelFRCollider").GetComponent<WheelCollider>();
		this.rlWheelCollider = transform.Find("WheelRL/WheelRLCollider").GetComponent<WheelCollider>();
		this.rrWheelCollider = transform.Find("WheelRR/WheelRRCollider").GetComponent<WheelCollider>();

		this.flWheelModel = this.transform.Find("WheelFL");
		this.frWheelModel = this.transform.Find("WheelFR");
		this.rlWheelModel = this.transform.Find("WheelRL");
		this.rrWheelModel = this.transform.Find("WheelRR");

		this.flDiscBrake = transform.Find("WheelFL/DiscBrakeFL");
		this.frDiscBrake = transform.Find("WheelFR/DiscBrakeFR");

		this.centerOfMass = transform.Find("CenterOfMass").transform;

		this.leftEmitter = transform.Find("LeftSkidSmoke").GetComponent<ParticleEmitter>();
		this.rightEmitter = transform.Find("RightSkidSmoke").GetComponent<ParticleEmitter>();

		this.leftLight = transform.Find("Lights/leftLight").gameObject;
		this.rightLight = transform.Find("Lights/rightLight").gameObject;
	}

	
}
