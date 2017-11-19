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

    [Header("Feedback to Player")]
    [SerializeField] private List<GameObject> _carrotsGameObjects = new List<GameObject>();

    [Header("Inventory Info")]
    [SerializeField] public bool HaveOpenSlot;
    [SerializeField] public bool IsFull;
    [Space]
    [SerializeField] private List<Slot> _inventorySlots = new List<Slot>();

    private void Awake()
    {
        HaveOpenSlot = CheckForNotBusySlot();
    }

    /// <summary>
    /// Tenta guardar algum item no primeiro slot nao ocupado que encontrar
    /// </summary>
    /// <param name="carrotId"></param>
    /// <returns></returns>
    public bool TryToStoreCarrot(int carrotId)
    {
        Slot slotToStore = _inventorySlots.Where(s => s.Busy == false).ToList().FirstOrDefault();

        if(slotToStore != null)
        {
            slotToStore.Store(carrotId);
            HaveOpenSlot = CheckForNotBusySlot();
            return true;
        }
        else
        {
            HaveOpenSlot = CheckForNotBusySlot();
            return false;
        }
    }

    /// <summary>
    /// Tenta retirar algum item no primeiro slot nao ocupado que encontrar
    /// </summary>
    /// <param name="carrotId"></param>
    /// <returns></returns>
    public int TryToWithdrawCarrot()
    {
        int carrotId = 0;

        Slot slotToStore = _inventorySlots.Where(s => s.Busy == true).ToList().FirstOrDefault();

        if (slotToStore != null)
        {
            carrotId = slotToStore.ItemId;
            slotToStore.Withdraw();
            HaveOpenSlot = CheckForNotBusySlot();
            return carrotId;
        }
        else
        {
            HaveOpenSlot = CheckForNotBusySlot();
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
    private bool CheckForNotBusySlot()
    {
        IsFull = !(_inventorySlots.Where(s => s.Busy == false).ToList().FirstOrDefault() != null);
        return !IsFull;
    }
}
