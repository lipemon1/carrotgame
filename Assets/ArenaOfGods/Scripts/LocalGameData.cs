using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalGameData : NetworkBehaviour {

	public static LocalGameData Instance { get; private set; }

    [SyncVar(hook = "OnSomeValueChanged")]
    public int SomeValue;

    [Header("Debug")]
    public Text ButtonText;

    private void Awake()
    {
        Instance = this;
    }

    public void AddLocalValue(int amountToAdd)
    {
        if (!isServer)
        {
            Debug.Log("Executando no servidor");
            //CmdOnSomeValueChanged(amountToAdd);
            SomeValue += amountToAdd;
        }
        else
        {
            Debug.Log("Executando no client");
            SomeValue += amountToAdd;
        }
    }

    private void OnSomeValueChanged(int valueFromServer)
    {
        SomeValue = valueFromServer;
        ButtonText.text = valueFromServer.ToString();
    }

    [Command]
    private void CmdOnSomeValueChanged(int valueToServer)
    {
        OnSomeValueChanged(valueToServer);
    }
}
