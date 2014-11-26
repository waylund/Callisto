using UnityEngine;
using System.Collections;

public class ShipControls : MonoBehaviour {

	public float speed = 25f;
	public float rotateSpeed = 1f;
	public float targetRange = 100f;
	public UnityEngine.UI.Text lockT, lightT;
	private bool isLocked = true;
	private Transform lockTarget;
	private int targetableMask;
	private Vector3 moveDirection = Vector3.zero;
	private Light engine1, engine2, engine3, engine4, spot1, spot2;

	// Use this for initialization
	void Start () {
		targetableMask = LayerMask.GetMask ("Crashable");
		Light[] engineLights = GetComponentsInChildren<Light> ();
		foreach (Light eL in engineLights) {
			switch (eL.name) {
				case "Engine1":
					engine1 = eL;
				break;
				case "Engine2":
					engine2 = eL;
				break;	
				case "Engine3":
					engine3 = eL;
				break;	
				case "Engine4":
					engine4 = eL;
				break;	
				case "Spot1":
					spot1 = eL;
				break;	
				case "Spot2":
					spot2 = eL;
				break;	
			}
		}
	}

	void Update () {

		// Turn lights On and Off
		if (Input.GetKeyDown ("l") || Input.GetKeyDown(KeyCode.Joystick1Button3)) {
			if (spot1.intensity > 0f)
			{
				spot1.intensity = 0f;
				spot2.intensity = 0f;
				lightT.text = "Spotlight: Off";
			}
			else {
				spot1.intensity = 5f;
				spot2.intensity = 5f;
				lightT.text = "Spotlight: On";
			}
		}

	}

	// Update is called once per frame
	void FixedUpdate () {

		// Calculate movement changes
		Move ();
		Turn ();
		if (Input.GetKeyDown ("space") || Input.GetKeyDown (KeyCode.Joystick1Button0)) 
		{
			if (isLocked) {
				lockTarget = null;
				isLocked = false;
				lockT.text = "Lock Target: None";
			} else {
				lockTarget = null;
				lockTarget = TakeAim ();
				isLocked = (lockTarget != null);
			}
		}

	}


	private Transform TakeAim()
	{
		Transform retObj = null;
		RaycastHit targetHit = new RaycastHit ();
		Ray targetRay = new Ray (transform.position, transform.forward);
		if (Physics.Raycast (targetRay, out targetHit, targetRange, targetableMask))
		{
			lockT.text = "Lock Target: " + targetHit.transform.name;
			retObj = targetHit.transform;
		}
		return retObj;
	}

	private void Move () {

		// Pull control inputs
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis ("ElevUp"), Input.GetAxis("Vertical"));

		// Merge Control Inputs with Current Direction
		moveDirection = rigidbody.transform.TransformDirection(moveDirection);

		// Multiply new force with speed
		moveDirection *= speed;

		// Apply Directional Force to the rigidbody
		rigidbody.AddForce (moveDirection, ForceMode.VelocityChange);

		if (Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKeyDown("x")) {
			rigidbody.velocity = new Vector3 (0f, 0f, 0f);
		}
	}

	private void Turn () {

		float turnSpd = rotateSpeed * Time.deltaTime;

		if (isLocked && lockTarget != null) {

			Vector3 targetDir = lockTarget.position - transform.position;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnSpd / 150, 10.0F);
			transform.rotation = Quaternion.LookRotation(newDir);

		}// else {

			///////////////////////////
			/// Get Input Values
			///////////////////////////

			float y = Input.GetAxis ("LookX");
			float x = Input.GetAxis ("LookY");
			if (y == 0) {
					if (Input.GetKey ("left"))
							y = -1;
					else if (Input.GetKey ("right"))
							y = 1;
			}
			if (x == 0) {
					if (Input.GetKey ("up"))
							x = -1;
					else if (Input.GetKey ("down"))
							x = 1;
			}

			// Apply input values to rotation
			transform.Rotate (x, y, 0f, Space.Self);

			///////////////////////////
			/// Manage Lights
			/// This should move to a new area
			///////////////////////////

			if (System.Math.Abs (y) < 0.9f) {
					y = 0;
					engine1.intensity = 0f;
					engine2.intensity = 0f;
					engine3.intensity = 0f;
					engine4.intensity = 0f;
			} else {
					y = y * turnSpd;
					if (y > 0) {
							engine3.intensity = 3f;
							engine4.intensity = 3f;
					} else {
							engine1.intensity = 3f;
							engine2.intensity = 3f;
					}
			}

			if (System.Math.Abs (x) < 0.9f) {
					x = 0;
					engine1.intensity = 0f;
					engine2.intensity = 0f;
					engine3.intensity = 0f;
					engine4.intensity = 0f;
			} else {
					x = x * turnSpd;
					if (x > 0) {
							engine1.intensity = 3f;
							engine3.intensity = 3f;
					} else {
							engine2.intensity = 3f;
							engine4.intensity = 3f;
					}
			}
		//}

	}
}
