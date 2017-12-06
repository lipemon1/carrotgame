using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunParticleBehaviour : MonoBehaviour {


    void OnEnable()
    {
        Invoke("Die", GetComponent<ParticleSystem>().main.duration);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Die()
    {
        Destroy(this.gameObject);
    }
}
