using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAudioTest : MonoBehaviour
{

    public bool planting;
    public float speed;
    
   

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    SoundManager.Instance.UpdateStolingAudio(planting);

        SoundManager.Instance.UpdateRunningAudio(speed);

	    if (Input.GetKeyDown(KeyCode.J))
	    {
	        SoundManager.Instance.Shoot();
	    }
	    if (Input.GetKeyDown(KeyCode.K))
	    {
	        SoundManager.Instance.Stun();
	    }
	    if (Input.GetKeyDown(KeyCode.L))
	    {
	        SoundManager.Instance.CarrotPlaced();
	    }
	    if (Input.GetKeyDown(KeyCode.M))
	    {
	        SoundManager.Instance.CarrotPickUp();
        }

    }
}
