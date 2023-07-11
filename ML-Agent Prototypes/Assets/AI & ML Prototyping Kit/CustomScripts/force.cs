using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class force : MonoBehaviour {

	public GameObject Circuit;
	public GameObject Car;
	public float InverseForceAmount=0.05f;
	public float Intensity;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Intensity=1/(InverseForceAmount*Vector3.Distance(Car.transform.position,Circuit.transform.position));
		Vector3 direction = (Circuit.transform.position - Car.transform.position).normalized;
		Circuit.GetComponent<Rigidbody>().AddForce(direction*Intensity);
		
		
	}
}
