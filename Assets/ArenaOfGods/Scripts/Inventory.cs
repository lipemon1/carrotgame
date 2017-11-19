using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory Instance { get; private set; }

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
    [Space]
    [SerializeField] private List<Slot> _inventorySlots = new List<Slot>();

    private void Awake()
    {
        Instance = this;
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
            CheckForNotBusySlot();
            return true;
        }
        else
        {
            CheckForNotBusySlot();
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
            CheckForNotBusySlot();
            return carrotId;
        }
        else
        {
            CheckForNotBusySlot();
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
        return _inventorySlots.Where(s => s.Busy == false).ToList().FirstOrDefault() != null;
    }
}
