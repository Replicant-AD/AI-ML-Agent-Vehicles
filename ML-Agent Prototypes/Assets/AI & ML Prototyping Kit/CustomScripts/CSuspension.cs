using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class CSuspension : MonoBehaviour {
	[Range(0, 20)]
	public float naturalFrequency = 10;

	[Range(0, 3)]
	public float dampingRatio = 0.8f;

	[Range(-1.5f, 1.5f)]
	public float VariableSpringConstant = 1f;

	public bool setSuspensionDistance = false;
	[Range(-1.5f, 1.5f)]
	public float GearShift;

	public bool DynamicForce = false;//WHEN THIS IS FALSE, THE CAR WILL REACT REALISTICALLY FOR SHORT DISTANCES AND LESS SPEED BUT BE LESS STABLE IN HIGH SPEEDS AND WILL TIP OVER IF VALUE IS CLOSE TO EXTREMES
							//IF THIS IS TRUE, THE CAR WILL BE MORE STABLE IN HIGH SPEEDS BUT WILL POSSESS LESS REALISM 

	[Range(-1f,1f)]
	public float ConstantForce; 

	[Range(1f,4f)]
	public float BrakeHardness;

	public Rigidbody Car;
	public float Speed;
	void Update () {
		// IF THE CAR GETS TOO JUMPY PLEASE DECREASE VARIABLE SPRING CONSTANT AND CHECK MARK THE DYNAMIC FORCE
		// IT CONTROLS FORCE APPLICATION POINT OF THE WHEEL COLLIDER, AS THE CAR GOES FASTER THE VALUE TENDS TO 0 SO THE CAR DOES NOT TIP OVER
		//Do not press space (Handbrake) and 'w' & 's' (accelerator) together. After handbrake reverse the car and then press accelerator for the gear shift
		foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>()) {
			JointSpring spring = wc.suspensionSpring;

			spring.spring = Mathf.Pow(Mathf.Sqrt(wc.sprungMass) * naturalFrequency, 2);
			spring.damper = 2 * dampingRatio * Mathf.Sqrt(spring.spring * wc.sprungMass);

			wc.suspensionSpring = spring;

			Vector3 wheelRelativeBody = transform.InverseTransformPoint(wc.transform.position);
			float distance = GetComponent<Rigidbody>().centerOfMass.y - wheelRelativeBody.y + wc.radius;

			Speed = Car.velocity.magnitude;
			if(DynamicForce == true)
			{
				GearShift=VariableSpringConstant/(Speed+1);
				wc.forceAppPointDistance = distance-GearShift;
			}
			if(DynamicForce == false)
			{
				wc.forceAppPointDistance = distance-ConstantForce;
			}
			
			if (spring.targetPosition > 0 && setSuspensionDistance)
				wc.suspensionDistance = wc.sprungMass * Physics.gravity.magnitude / (spring.targetPosition * spring.spring);

			WheelFrictionCurve fFriction = wc.forwardFriction;
			fFriction.stiffness = BrakeHardness;
			wc.forwardFriction = fFriction;
		}
	}
}
