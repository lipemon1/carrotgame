using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour {

    [Header("Debug")]
    [SerializeField] private Collider _targetToPick;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carrot"))
        {
            Debug.Log("Colliding with some carrot: " + other.gameObject.name);
            _targetToPick = other;
            _targetToPick.GetComponent<CarrotBehaviour>().OnTargetEnter();
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
            }
        }
    }
}
