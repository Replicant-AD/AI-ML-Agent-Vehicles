using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sensors : MonoBehaviour {
	//public GameObject Truck;
	public float Sensorlength = 1000f;
	public Vector3 frontSensorPos ;
	public float frontSideSensorPos = 1.5f;
	public float frontSensorAngle=30f;
	public float sensorSpeed =20f;
	public float x;
	public Text text;
	public Text text1;
	public Text text2;
	public Text text3;
	public Text text4;
	public Text text5;
	public Text text6;
	public Text text7;
	public Text text8;
	
	void Update () {
		//Debug.DrawLine(new Vector3(Truck.transform.position.x,transform.position.y,transform.position.z), new Vector3(1,1,1), Color.green);
		RaycastHit hit;
		x+=Time.deltaTime;
		if(x>=0f&&x<2f)
		{
			frontSensorAngle += Time.deltaTime*sensorSpeed;
		}
		else if(x>=2f&&x<4f)
		{
			frontSensorAngle-= Time.deltaTime*sensorSpeed;
		
		}
		if(x>=4f)
		x=0f;
		Vector3 sensorstartpos=transform.position;
		sensorstartpos += transform.forward* frontSensorPos.z;
		sensorstartpos += transform.up * frontSensorPos.y;
		if(Physics.Raycast(sensorstartpos, transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.green);
		}

		sensorstartpos+= transform.right*frontSideSensorPos;
		if(Physics.Raycast(sensorstartpos, transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.red);
		}

		if(Physics.Raycast(sensorstartpos, Quaternion.AngleAxis(frontSensorAngle,transform.up)*transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.white);
		}
		if(Physics.Raycast(sensorstartpos, Quaternion.AngleAxis(frontSensorAngle+10f,transform.up)*transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.white);
		}

		if(Physics.Raycast(sensorstartpos, Quaternion.AngleAxis(frontSensorAngle+20f,transform.up)*transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.white);
			text6.text=hit.point.x.ToString();
		text7.text=hit.point.y.ToString();
		text8.text=hit.point.z.ToString();
		}

		sensorstartpos-= transform.right*frontSideSensorPos*2;
		if(Physics.Raycast(sensorstartpos, transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.red);
		}

		
		if(Physics.Raycast(sensorstartpos, Quaternion.AngleAxis(-frontSensorAngle,transform.up)*transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.white);
		}
		
		if(Physics.Raycast(sensorstartpos, Quaternion.AngleAxis(-frontSensorAngle-10f,transform.up)*transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.white);
		}

		if(Physics.Raycast(sensorstartpos, Quaternion.AngleAxis(-frontSensorAngle-20f,transform.up)*transform.forward, out hit, Sensorlength))
		{
			Debug.DrawLine(sensorstartpos, hit.point,Color.white);
		}

		text.text=hit.point.x.ToString();
		text1.text=hit.point.y.ToString();
		text2.text=hit.point.z.ToString();

		text3.text=sensorstartpos.x.ToString();
		text4.text=sensorstartpos.y.ToString();
		text5.text=sensorstartpos.z.ToString();

		
		
		
	}
}
