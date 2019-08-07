using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Carrot : MonoBehaviour{

	[Header("Carrot Game Instance")]
    [SerializeField] public GameObject GameInstance;

    [Header("Info")]
    [SerializeField] public int Id;
    [SerializeField] public int PlayerOwnerId = -1;

    [SerializeField] public bool IsActive;

    private void Start()
    {
        if (GameInstance == null)
            GameInstance = this.gameObject;

        IsActive = this.gameObject.activeInHierarchy;
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
