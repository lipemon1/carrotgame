using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupLocalPlayer : NetworkBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Scripts")]
    [SerializeField] public PlayerMovement PlayerMovement;
    [SerializeField] public ItemPicker ItemPicker;
    [SerializeField] public ItemDropper ItemDropper;
    [SerializeField] public Inventory Inventory;
    [SerializeField] public ActionButtonHandler ActionButtonHandler;
    [SerializeField] public GameData GameData;
    [SerializeField] public PlayerIdentity PlayerIdentity;

    [Header("Spawn Configs")]
    [SerializeField] private float _spawnYOffset = 5f;

	// Use this for initialization
	void Start () {

        ConfigureAllScripts();

        transform.SetParent(GameObject.Find("Arena").gameObject.transform);
        transform.position = new Vector3(transform.position.x, transform.position.y + _spawnYOffset, transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ConfigureAllScripts()
    {
        if (isLocalPlayer)
        {
            PlayerMovement.enabled = true;
            //ItemPicker.enabled = true;
            //ItemDropper.enabled = true;
            //Inventory.enabled = true;
            //ActionButtonHandler.enabled = true;
            //PlayerIdentity.enabled = true;

            if (_showDebugMessages) Debug.Log("Configurando setup local player de: " + gameObject.name);
        }
        else
        {
            PlayerMovement.enabled = false;
            //ItemPicker.enabled = false;
            //ItemDropper.enabled = false;
            //Inventory.enabled = false;
            //ActionButtonHandler.enabled = false;
            //PlayerIdentity.enabled = false;
        }
    }

    public void SetMyId()
    {
        //PlayerIdentity.SetPlayerId(myId);
    }
}
