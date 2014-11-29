using UnityEngine;
using System.Collections;

public class WeaponShooting : MonoBehaviour {

	public float weaponDamagePerShot = 0f;
	public float weaponRange = 0f;
	public float fireRate = 2f;
	public float weaponForce = 0f;
	public float impactSize = 0f;

	float timer;
	int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
	LineRenderer gunLine;                           // Reference to the line renderer.
	AudioSource gunAudio;                           // Reference to the audio source.
	Light gunLight;                                 // Reference to the light component.
	float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
	Ray shootRay;
	RaycastHit shootHit;

	void Awake ()
	{
		// Create a layer mask for the Shootable layer.
		shootableMask = LayerMask.GetMask ("Crashable");
		
		// Set up the references.
		gunLine = GetComponent <LineRenderer> ();
		gunAudio = GetComponent<AudioSource> (); 
		gunLight = GetComponent<Light> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if ((Input.GetButton ("Fire1") || Input.GetAxis("FireTrigger") > 0) && timer > fireRate)
			Fire ();

		if(timer >= fireRate * effectsDisplayTime)
		{
			// ... disable the effects.
			DisableEffects ();
		}
	}

	private void Fire () {
		// Reset the timer.
		timer = 0f;
		
		// Play the gun shot audioclip.
		gunAudio.Play (); 
		
		// Enable the light.
		gunLight.enabled = true;
		
		// Enable the line renderer and set it's first position to be the end of the gun.
		gunLine.enabled = true;
		gunLine.SetPosition (0, transform.position);
		
		// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
		shootRay.origin = transform.position;
		shootRay.direction = transform.forward;
		
		// Perform the raycast against gameobjects on the shootable layer and if it hits something...
		if(Physics.Raycast (shootRay, out shootHit, weaponRange, shootableMask))
		{
			// Try and find an EnemyHealth script on the gameobject hit.
			ShootableHealth enemyHealth = shootHit.collider.GetComponent <ShootableHealth> ();
			
			// If the EnemyHealth component exist...
			if(enemyHealth != null)
			{
				// ... the enemy should take damage.
				enemyHealth.TakeDamage (weaponDamagePerShot, shootHit.point, weaponForce, impactSize);
			}
			
			// Set the second position of the line renderer to the point the raycast hit.
			gunLine.SetPosition (1, shootHit.point);
		}
		// If the raycast didn't hit anything on the shootable layer...
		else
		{
			// ... set the second position of the line renderer to the fullest extent of the gun's range.
			gunLine.SetPosition (1, shootRay.origin + shootRay.direction * weaponRange);
		}

	}

	private void DisableEffects () {
		gunLine.enabled = false;
		gunLight.enabled = false;
	}


}
