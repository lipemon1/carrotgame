using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {

    [System.Serializable]
    public class Slot
    {
        public bool Busy;
        public int ItemId = -1;

        public void Store(int itemId)
        {
            ItemId = itemId;
            Busy = true;
        }

        public void Withdraw()
        {
            ItemId = -1;
            Busy = false;
        }
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Feedback to Player")]
    [SerializeField] private List<GameObject> _carrotsGameObjects = new List<GameObject>();

    [Header("Inventory Info")]
    [SerializeField] public bool HaveOpenSlot;
    [SerializeField] public bool IsFull;
    [SerializeField] public bool HaveSomeCarrot;
    [Space]
    [SerializeField] private List<Slot> _inventorySlots = new List<Slot>();

    private void Awake()
    {
        HaveOpenSlot = UpdateStatusCheckers();
    }

    /// <summary>
    /// Tenta guardar algum item no primeiro slot nao ocupado que encontrar
    /// </summary>
    /// <param name="carrotId"></param>
    /// <returns></returns>
    public bool TryToStoreCarrot(int carrotId)
    {
        if (_showDebugMessages) Debug.Log("Iniciando store de cenoura no inventário");

        Slot slotToStore = _inventorySlots.Where(s => s.Busy == false).ToList().FirstOrDefault();

        if(slotToStore != null)
        {
            if (_showDebugMessages) Debug.Log("Store disponível encontrado");
            slotToStore.Store(carrotId);
            HaveOpenSlot = UpdateStatusCheckers();
            return true;
        }
        else
        {
            HaveOpenSlot = UpdateStatusCheckers();
            return false;
        }
    }

    /// <summary>
    /// Tenta retirar algum item no primeiro slot nao ocupado que encontrar, retorna o id do item que foi retirado, retorna -1 se não conseguiu tirar nenhum item
    /// </summary>
    /// <param name="carrotId"></param>
    /// <returns></returns>
    public int TryToWithdrawCarrot()
    {
        if (_showDebugMessages) Debug.Log("Iniciando retirada de item do inventário");
        int carrotId = 0;

        Slot slotToStore = _inventorySlots.Where(s => s.Busy == true).ToList().FirstOrDefault();

        if (slotToStore != null)
        {
            carrotId = slotToStore.ItemId;
            if (_showDebugMessages) Debug.Log("Item a ser retirado com ID:" + carrotId);

            slotToStore.Withdraw();
            HaveOpenSlot = UpdateStatusCheckers();
            return carrotId;
        }
        else
        {
            HaveOpenSlot = UpdateStatusCheckers();
            return -1;
        }
    }

    /// <summary>
    /// Atualiza as cenouras no personagem de acordo com os slots do personagem
    /// </summary>
    private void UpdateCarrotsFeedback()
    {
        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            _carrotsGameObjects[i].SetActive(_inventorySlots[i].Busy);
        }
    }

    /// <summary>
    /// Verifica se existe um slot disponivel para ser usado
    /// </summary>
    /// <returns></returns>
    private bool UpdateStatusCheckers()
    {
        HaveSomeCarrot = (_inventorySlots.Where(s => s.Busy == true).ToList().FirstOrDefault() != null);

        IsFull = !(_inventorySlots.Where(s => s.Busy == false).ToList().FirstOrDefault() != null);
        return !IsFull;
    }
}
