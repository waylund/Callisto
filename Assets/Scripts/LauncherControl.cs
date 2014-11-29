using UnityEngine;
using System.Collections;

public class LauncherControl : MonoBehaviour {

	public GameObject projectile;
	public int ammo;
	float timer = 5f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;

		if (Input.GetButton ("Fire2") && ammo > 0 && timer > 5f) 
		{
			timer = 0;
			Instantiate(projectile, transform.position, transform.rotation);
			ammo--;
		}
	}
}
