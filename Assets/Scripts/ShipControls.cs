using UnityEngine;
using System.Collections;

public class ShipControls : MonoBehaviour {

	public float speed = 25f;
	public float rotateSpeed = 1f;
	public float targetRange = 100f;
	public UnityEngine.UI.Text lockT, lightT, torpT, thT;
	private bool isLocked = false;
	private Transform lockTarget;
	private int targetableMask;
	private Vector3 moveDirection = Vector3.zero;
	private Light engine1, engine2, engine3, engine4, spot1, spot2;
	private LauncherControl launcher;
	private ShootableHealth targetHealth;
	float timer = 0f;
	float betweenLock = .5f;


	// Use this for initialization
	void Start () {
		targetableMask = LayerMask.GetMask ("Crashable");
		launcher = GetComponentInChildren<LauncherControl> ();
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
		if (isLocked) 
		{
			double Thealth = System.Math.Floor (targetHealth.health);
			if (Thealth > 0)
				thT.text = "Target Structure: " + System.Math.Floor (targetHealth.health);
			else
				thT.text = "Target Structure: Destroyed";
		}
		torpT.text = "Torpedos: " + launcher.ammo;
		timer += Time.deltaTime;
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
		if (Input.GetButton ("Lock") && timer > betweenLock) 
		{
			timer = 0f;

			if (isLocked) {
				lockTarget = null;
				isLocked = false;
				lockT.text = "Lock Target: None";
				thT.text = "";
			} else {
				lockTarget = null;
				lockTarget = TakeAim ();
				if (lockTarget != null)
				{
					isLocked = true;
					lockT.text = "Lock Target: " + lockTarget.name;
					targetHealth = lockTarget.GetComponent<ShootableHealth>();
					thT.text = "Target Structure: " + System.Math.Floor(targetHealth.health);
				} else isLocked = false;
			}
		}
	}

	public Transform getTarget()
	{
		return lockTarget;
	}

	private Transform TakeAim()
	{
		Transform retObj = null;
		RaycastHit targetHit = new RaycastHit ();
		Ray targetRay = new Ray (transform.position, transform.forward);
		if (Physics.Raycast (targetRay, out targetHit, targetRange, targetableMask))
		{
			retObj = targetHit.transform;
		}
		return retObj;
	}

	// Execute Ship Movement
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

	// Execute Directional Turning
	private void Turn () {

		float turnSpd = rotateSpeed * Time.deltaTime;

		if (isLocked && lockTarget != null) {

			Vector3 targetDir = lockTarget.position - transform.position;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnSpd / 150, 10.0F);
			transform.rotation = Quaternion.LookRotation(newDir);

		}

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


	}
}
