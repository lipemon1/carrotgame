using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ArenaTrackableEventHandler : MonoBehaviour, ITrackableEventHandler {

    private TrackableBehaviour mTrackableBehaviour;

    [Header("Lobby Canvas")]
    [SerializeField] private GameObject _lobbyCanvas;

    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    #region PUBLIC_METHODS

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS


    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        MeshRenderer[] meshRendererComponents = GetComponentsInChildren<MeshRenderer>(true);
        //Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable Mesh rendering:
        foreach (MeshRenderer component in meshRendererComponents)
        {
            component.enabled = true;
        }

        //// Enable colliders:
        //foreach (Collider component in colliderComponents)
        //{
        //    component.enabled = true;
        //}

        //showing lobby canvas
        _lobbyCanvas.gameObject.SetActive(true);

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    }


    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        MeshRenderer[] meshRendererComponents = GetComponentsInChildren<MeshRenderer>(true);
        //Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Disable Mesh rendering:
        foreach (MeshRenderer component in meshRendererComponents)
        {
            component.enabled = false;
        }

        // Disable colliders:
        //foreach (Collider component in colliderComponents)
        //{
        //    component.enabled = false;
        //}

        //showing lobby canvas
        _lobbyCanvas.gameObject.SetActive(false);

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }

    #endregion // PRIVATE_METHODS
}
