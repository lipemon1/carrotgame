using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerConnected {

    [SyncVar]
    public int PlayerId;

    [SyncVar]
    public bool IsReady;

    [SyncVar]
    public bool IsConnected;

    [SyncVar]
    public string PlayerName;

    [SyncVar]
    public GameObject GameInstance;
}
