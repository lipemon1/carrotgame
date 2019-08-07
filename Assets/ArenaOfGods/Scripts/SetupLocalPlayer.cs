using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupLocalPlayer : MonoBehaviour {

    [System.Serializable]
    public struct SetupConfig
    {
        public bool PlayerMovement;
        public bool ItemPicker;
        public bool ItemDropper;
        public bool Inventory;
        public bool ActionButtonHandler;
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Stay active")]
    [SerializeField] private SetupConfig _componentsToActive;

    [Header("Scripts")]
    [SerializeField] public PlayerMovement PlayerMovement;
    [SerializeField] public ItemPicker ItemPicker;
    [SerializeField] public ItemDropper ItemDropper;
    [SerializeField] public Inventory Inventory;
    [SerializeField] public ActionButtonHandler ActionButtonHandler;
    [SerializeField] public int PlayerId;

    [Header("Spawn Configs")]
    [SerializeField] private float _spawnYOffset = 5f;

    [Header("Materials")]
    [SerializeField] private SkinnedMeshRenderer _playerMeshRenderer;

    public void ConfigureAllScripts()
    {
        if (_showDebugMessages) Debug.Log("Iniciando configuração de setup: " + gameObject.name);

        if(_componentsToActive.PlayerMovement) PlayerMovement.enabled = true;
        if (_componentsToActive.ItemPicker) ItemPicker.enabled = true;
        if (_componentsToActive.ItemDropper) ItemDropper.enabled = true;
        if (_componentsToActive.Inventory) Inventory.enabled = true;
        if (_componentsToActive.ActionButtonHandler) ActionButtonHandler.enabled = true;

        if (_showDebugMessages) Debug.Log("Configuração realizada de setup local player de: " + gameObject.name);

        transform.SetParent(GameObject.Find("Arena").gameObject.transform);
        transform.position = new Vector3(transform.position.x, transform.position.y + _spawnYOffset, transform.position.z);
    }

    /// <summary>
    /// Altera o material que está sendo usado pelo jogador
    /// </summary>
    /// <param name="newMaterial"></param>
    public void ChangePlayerMaterial(Material newMaterial)
    {
        Material[] mats = new Material[] { newMaterial};

        _playerMeshRenderer.materials = mats;
    }
}
