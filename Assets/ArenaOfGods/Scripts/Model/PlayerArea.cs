using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerArea : MonoBehaviour{

	[Header("Player Area Instance")]
    [SerializeField] public GameObject GameInstance;

    [Header("Info")]
    [SerializeField] public int Id;
    [SerializeField] private Material _materialToPlayer;
    [SerializeField] private Color _playerColor;

    [SerializeField] public int PlayerOwnerId;
    [SerializeField] public bool IsActive;

    [Header("Carrots List")]
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    [Header("Interface")]
    [SerializeField] private Text _carrotsAmountUI;

    private void Start()
    {
        if (GameInstance == null)
            GameInstance = this.gameObject;

        IsActive = this.gameObject.activeInHierarchy;
    }

    public Material GetMaterialToPlayer() { return _materialToPlayer; }

    public Color GetPlayerColor() { return _playerColor; }

    public void UpdateInterface()
    {
//        _carrotsAmountUI.text = CarrotsList.Count.ToString();
    }

    #region PlayerOwnerChange

    private void ChangePlayerOwnerId(int id)
    {
        Debug.Log("Mudando dono da cenoura: " + Id);
        PlayerOwnerId = id;
    }
    #endregion

    #region ActiveChange

    private void ChangeActiveValue(bool value)
    {
        Debug.Log("Changing Active");
        IsActive = value;
        GameInstance.SetActive(IsActive);
    }
    #endregion
}
