using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class HelicopterCamera : MonoBehaviour
	{

		// The target we are following
		[SerializeField]
		private Transform target;					
		// The distance in the x-z plane to the target
		public float distance = 8 ;
		
		// the height we want the camera to be above the target
		[SerializeField]
		private float height = 5.0f;

		[SerializeField]
		private float rotationDamping;
		[SerializeField]
		private float heightDamping =1f;
		public float perlinnoise =0f;
		public float perlinnoise2 =0f;
		private float elapsedtime = 0f;
		public float multiplier = 0.5f;
		public float timemultiplier = 5f;
		public int inttime;
		public bool HelicopterFeatureEnable=false;
		public float elapsedreset;
		public float shakeAmount=2f;
		public float shakeFreq=2f;
		public float shakeAmountReset=3f;
		public float shakeFreqReset=3f;
		public float shakeFreqBoost=0.08f;
		public float limitBoost=5f;
		public bool lookat;

		// Use this for initialization
		void Start() { }

		// Update is called once per frame
		void LateUpdate()
		{
			// Early out if we don't have a target
			if (!target)
				return;

			// Calculate the current rotation angles
			var wantedRotationAngle = target.eulerAngles.y;
			var wantedHeight = target.position.y + height;

			var currentRotationAngle = transform.eulerAngles.y;
			var currentHeight = transform.position.y; 

			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

			// Damp the height
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			// Convert the angle into a rotation
			var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;

			// Set the height of the camera
			var d = Input.GetAxis("Mouse ScrollWheel");
			if(d>0f)
			{
				height=height+1;
			}
			else if (d<0f)
			{
				height=height-1;
			}
			if(Input.GetKey("z"))
			{
				distance=distance-Time.deltaTime;
			}
			else if (Input.GetKey("x"))
			{
				distance=distance+Time.deltaTime;
			}

			if(HelicopterFeatureEnable==true)
			{
			elapsedreset = Time.deltaTime;
			if(Input.GetKey("w"))
			{	
				timemultiplier+=elapsedreset/shakeFreq;
				multiplier+=elapsedreset/shakeAmount;			
			}
			if(Input.GetKey("w")&&Input.GetKey(KeyCode.LeftShift)&&timemultiplier<limitBoost)
			{	
				timemultiplier+=elapsedreset/shakeFreqBoost;
				multiplier-=elapsedreset/(shakeAmount*3f);			
			}
			if(timemultiplier > 0)
			{
				timemultiplier-=elapsedreset/shakeFreqReset;
				
			}
			 if(multiplier>0)
			{
				multiplier-=elapsedreset/shakeAmountReset;
			}
			else
			{
				elapsedreset=0;
			}

			elapsedtime = Time.time;
			perlinnoise = Mathf.PerlinNoise(timemultiplier,0)-0.5f;
			perlinnoise2 = Mathf.PerlinNoise(timemultiplier,elapsedtime)-0.5f;
			transform.position = new Vector3(transform.position.x+(((perlinnoise)*multiplier)) , currentHeight+perlinnoise2*multiplier , transform.position.z);

			}
			else
			{
				transform.position = new Vector3(transform.position.x , currentHeight , transform.position.z);
			}
			if(lookat==true)
			{
				transform.LookAt(target);
			}
			
		}
	}
}