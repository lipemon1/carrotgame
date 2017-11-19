﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// Retorna verdadeiro se o jogador passado por parametro é o dono da area passada por parametro
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="playerAreaId"></param>
    /// <returns></returns>
	public bool IsThisMyArea(int playerId, int playerAreaId)
    {
        return PlayerAreaList.Where(pa => pa.Id == playerAreaId && pa.PlayerOwnerId == playerId).First() != null;
    }
}
