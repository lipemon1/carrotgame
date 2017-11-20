using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OverridenNetworkDiscovery : NetworkDiscovery {

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        this.StopBroadcast();
        base.OnReceivedBroadcast(fromAddress, data);
        Debug.Log("Encontrado o ip: " + fromAddress);
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
    }
}
