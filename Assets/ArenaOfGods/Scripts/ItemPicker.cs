using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour {

    [Header("Debug")]
    [SerializeField] private CarrotIdentity _targetToPick;
    [SerializeField] public bool IsTouchingCarrot;

    [Header("Scripts")]
    [SerializeField] private ActionButtonHandler _actionButtonHandler;

    private void Awake()
    {
        if (_actionButtonHandler == null)
            _actionButtonHandler.GetComponent<ActionButtonHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carrot"))
        {
            Debug.Log("Colliding with some carrot: " + other.gameObject.name);
            _targetToPick = other.GetComponent<CarrotIdentity>();
            _targetToPick.GetComponent<CarrotBehaviour>().OnTargetEnter();
            IsTouchingCarrot = true;
            _actionButtonHandler.CheckButtonToShow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Carrot"))
        {
            if (_targetToPick != null)
            {
                _targetToPick.GetComponent<CarrotBehaviour>().OnTargetExit();
                _targetToPick = null;
                IsTouchingCarrot = false;
                _actionButtonHandler.CheckButtonToShow();
            }
        }
    }
}
