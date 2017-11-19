using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Carrot {

	[Header("Carrot Game Instance")]
    [SerializeField] public GameObject GameInstance;

    [Header("Info")]
    [SerializeField] public int Id;
    [SerializeField] public string PlayerOwnerId;
}
