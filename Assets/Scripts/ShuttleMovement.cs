using UnityEngine;
using System.Collections;

public class ShuttleMovement : MonoBehaviour {

	public float moveSpeed = 10f;
	int i = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//if (i < 20) {
						// Pull control inputs
						Vector3 moveDirection = new Vector3 (moveSpeed, 0f, 0f);
			
						// Merge Control Inputs with Current Direction
						moveDirection = transform.TransformDirection (moveDirection);
			
						// Multiply new force with speed
						moveDirection *= moveSpeed + Time.deltaTime;
			
						// Apply Directional Force to the rigidbody
						rigidbody.AddForce(moveDirection);

						i++;
				//}
	}
}
