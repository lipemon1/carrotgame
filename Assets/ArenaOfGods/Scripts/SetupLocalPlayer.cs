using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {

    [Header("Scripts")]
    [HideInInspector] private PlayerMovement _playerMovement;

    [Header("Spawn Configs")]
    [SerializeField] private float _spawnYOffset = 5f;

	// Use this for initialization
	void Start () {
        transform.SetParent(GameObject.Find("ImageTarget").gameObject.transform);
        transform.position = new Vector3(transform.position.x, transform.position.y + _spawnYOffset, transform.position.z);
        _playerMovement = GetComponent<PlayerMovement>();
        ;

        if (isLocalPlayer)
        {
            _playerMovement.enabled = true;
        }
        else
        {
            _playerMovement.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
