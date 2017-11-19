using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentity : MonoBehaviour {

    public int PlayerId = -1;

    /// <summary>
    /// Altera o valor do id do jogador
    /// </summary>
    /// <param name="value"></param>
    public void SetPlayerId(int value)
    {
        PlayerId = value;
    }
}
