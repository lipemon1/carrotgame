using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerArea {

	[Header("Player Area Instance")]
    [SerializeField] public GameObject GameInstance;

    [Header("Info")]
    [SerializeField] public int Id;
    [SerializeField] public string PlayerOwnerId;
    [SerializeField] public List<int> CarrotsListId = new List<int>();
}
