using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private GeneralEvent _playerDeath;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tags.ORB_TAG)
        {
            OrbMovementStateMachine orbMovementStateMachine = other.GetComponentInParent<OrbMovementStateMachine>();
            if(orbMovementStateMachine.Catched()) { return; }

            Debug.Log("Death");
            _playerDeath.Raise();
        }
    }
}
