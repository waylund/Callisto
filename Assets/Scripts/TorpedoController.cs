using UnityEngine;
using System.Collections;

public class TorpedoController : MonoBehaviour {

	public float maxTime = 10f; 	// number of seconds until automatic detonation
	public GameObject explosion;
	public float explodeForce;
	public float explodeRadius;
	public float moveSpeed = 10f;
	public float weaponDamage = 200f;
	Transform target;

	private float timer = 0f;

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindWithTag ("Player");
		ShipControls controls = player.GetComponent<ShipControls> ();
		target = controls.getTarget ();


	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if (target != null) 
		{
			Vector3 targetDir = target.position - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 10f / 150, 10.0F);
			transform.rotation = Quaternion.LookRotation (newDir);
		}

		Vector3 moveDirection = new Vector3 (0f, 0f, moveSpeed);
		moveDirection = transform.TransformDirection (moveDirection);
		moveDirection *= moveSpeed + Time.deltaTime;
		rigidbody.AddForce(moveDirection);

		if (timer > maxTime)
			Explode ();
	}

	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		ShootableHealth enemyHealth = other.GetComponent <ShootableHealth> ();
		
		// If the EnemyHealth component exist...
		if(enemyHealth != null)
		{
			// ... the enemy should take damage.
			enemyHealth.TakeDamage (weaponDamage, transform.position, 0f, 0f);
		}

		Explode ();
	}

	void Explode() {
		Vector3 bangPoint = transform.position;
			
		if (explosion != null)
			Instantiate (explosion, bangPoint, new Quaternion ());
		
		Collider[] colliders = Physics.OverlapSphere (bangPoint, explodeRadius);
		
		foreach (Collider thisCollider in colliders) 
		{
			if (thisCollider.rigidbody == null) continue;
			
			thisCollider.rigidbody.AddExplosionForce(explodeForce, bangPoint, explodeRadius, 0, ForceMode.Impulse);
		}
		
		Destroy (gameObject);
	}
}
