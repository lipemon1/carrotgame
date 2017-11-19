using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    public static GameData Instance { get; private set; }

    [Header("Game Data")]
    [SerializeField] public PlayerArea InitialArea;
    [SerializeField] public List<PlayerArea> PlayerAreaList = new List<PlayerArea>();
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    void Awake()
    {
        Instance = this;
    }

	
}
