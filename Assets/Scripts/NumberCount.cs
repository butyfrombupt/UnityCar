using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCount : MonoBehaviour {

	// Use this for initialization
	public int startCount = 300;
	public UILabel timeCount;
	void Start () {
		StartCoroutine(Count());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	IEnumerator Count() {
		timeCount.text = startCount.ToString ();
		while (startCount > 0) {
			yield return new WaitForSeconds (1f);
			startCount--;
			timeCount.text = startCount.ToString ();
		}
		yield return new WaitForSeconds (0.1f);
		//这里留着写超出时间怎么办
	}

}
