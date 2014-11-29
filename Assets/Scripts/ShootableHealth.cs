using UnityEngine;
using System.Collections;

public class ShootableHealth : MonoBehaviour {

	public float health = 0f;
	public float regenRate = 0f;
	public float dieForce = 0f;
	public float dieRad = 0f;
	public GameObject explosionEffect;
	private bool isDead = false;
	private float maxHealth = 0f;
	private ParticleSystem hitParticles;


	// Use this for initialization
	void Awake () {
		maxHealth = health;
		hitParticles = GetComponentInChildren<ParticleSystem> ();
	}

	// This method is called to trigger a death animation and remove the object from the game
	void Die () {
		isDead = true;
		Debug.Log ("Target Destroyed: " + transform.name);

		Vector3 bangPoint = transform.position;


		if (explosionEffect != null)
			Instantiate (explosionEffect, bangPoint, new Quaternion ());

		Collider[] colliders = Physics.OverlapSphere (bangPoint, dieRad);

		foreach (Collider thisCollider in colliders) 
		{
			if (thisCollider.rigidbody == null) continue;

			thisCollider.rigidbody.AddExplosionForce(dieForce, bangPoint, dieRad, 0, ForceMode.Impulse);
		}

		Destroy (gameObject, .5f);

	}

	void Regen () {
		if (!isDead) {
			health += regenRate * Time.deltaTime;
		}
	}

	public void TakeDamage (float amount, Vector3 hitPoint) 
	{
		TakeDamage (amount, hitPoint, 0f, 0f);
	}

	public void TakeDamage (float amount, Vector3 hitPoint, float weaponForce, float impactSize)
	{
		if(isDead)
			// ... no need to take damage so exit the function.
			return;
		
		// Reduce the current health by the amount of damage sustained.
		health -= amount;
		
		// Set the position of the particle system to where the hit was sustained.
		hitParticles.transform.position = hitPoint;
		
		// And play the particles.
		hitParticles.Play();

		rigidbody.AddExplosionForce (weaponForce, hitPoint, impactSize, 0, ForceMode.Force);

		Debug.Log ("Took Hit: " + amount + "damage. Remaining: " + health);
		// If the current health is less than or equal to zero...
		if(health <= 0)
		{
			// ... the enemy is dead.
			Die ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (health <= 0) 
			Die ();
		else if (health < maxHealth)
			Regen ();
	}
}
