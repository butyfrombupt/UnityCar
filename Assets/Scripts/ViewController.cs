using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
public class ViewController : MonoBehaviour {

	// Use this for initialization
	public bool isFirstView = false;
	public GameObject mainCamera;
	public GameObject firstViewCamera;

	void Start () {
		
	}


	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.V)) {
			if (isFirstView) {
				mainCamera.SetActive(true);
				firstViewCamera.SetActive(false);

				isFirstView = false;
			} else {
				mainCamera.SetActive(false);
				firstViewCamera.SetActive(true);

				isFirstView = true;
			}
		}

	}
}
