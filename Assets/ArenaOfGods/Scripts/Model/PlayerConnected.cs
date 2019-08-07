using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerConnected {

    public int PlayerId;

    public bool IsReady;

    public bool IsConnected;

    public string PlayerName;

    public GameObject GameInstance;
}
