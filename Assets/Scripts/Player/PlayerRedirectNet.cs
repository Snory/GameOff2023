using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRedirectNet : MonoBehaviour
{
    public UnityEvent<OrbMovementStateMachine> SelectedTrackObject;
    public UnityEvent<OrbMovementStateMachine> DeselectedTrackObject;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != Tags.ORB_TAG) return;

        SelectedTrackObject?.Invoke(other.GetComponentInParent<OrbMovementStateMachine>());  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != Tags.ORB_TAG) return;

        DeselectedTrackObject?.Invoke(other.GetComponentInParent<OrbMovementStateMachine>());
    }


}
