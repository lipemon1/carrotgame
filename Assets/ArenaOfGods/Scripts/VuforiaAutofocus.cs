using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class VuforiaAutoFocus : MonoBehaviour {
    public bool autoFocusOnClick = true;
    public bool autoFocusOnTimer = true;
    // Time in secounds
    public float autoFocusTimer = 3f;

    private bool hasAutoFocus = false;

    private void OnEnable()
    {
        autoFocusTimer = Mathf.Clamp(autoFocusTimer, 0, Mathf.Infinity);
        Invoke("LateEnable", 1f);
    }

    private void LateEnable ()
	{
        hasAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        StopAllCoroutines();
        if (!hasAutoFocus)
            StartCoroutine("AutoFocusTimer");
    }

	private void Update(){
		if (autoFocusOnClick && Input.GetMouseButtonDown(0))
            TriggerAutoFocus();
    }

	IEnumerator AutoFocusTimer(){
        StateManager stateManager = TrackerManager.Instance.GetStateManager();
        while (true) {
            bool hasActiveTrackable = false;
            foreach (TrackableBehaviour activeTrackable in stateManager.GetActiveTrackableBehaviours())
                hasActiveTrackable = true;

            if(!hasActiveTrackable)
                TriggerAutoFocus();
            yield return new WaitForSeconds(autoFocusTimer);
		}
	}

    private void TriggerAutoFocus()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
        if(hasAutoFocus)
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }
}
