using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControlle : MonoBehaviour
{

    public CharAnimationsController anim;
    public float speed;
    public bool WithGun;
    public bool planting;
    public bool stoling;

    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    anim.UpdateAnimations(speed, planting, stoling, WithGun);
	}

    public void Shoot()
    {
        anim.Shoot();
    }

    void OnValidate()
    {
        
    }
}
