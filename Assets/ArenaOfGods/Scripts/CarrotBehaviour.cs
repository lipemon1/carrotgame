using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBehaviour : MonoBehaviour {

    [Header("Components")]
    [SerializeField] private SpriteRenderer _targetMarker;

    private void Start()
    {
        if (_targetMarker.enabled)
            _targetMarker.enabled = false;
    }

	public void OnTargetEnter()
    {
        _targetMarker.enabled = true;
    }

    public void OnTargetExit()
    {
        _targetMarker.enabled = false;
    }

    public void OnSomeonePicked(int playerId)
    {

    }
}
