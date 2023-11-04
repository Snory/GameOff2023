using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class OrbTracker : MonoBehaviour
{
    private Dictionary<OrbMovementStateMachine, bool> _trackedOrbs;

    [SerializeField]
    private int _maxOrbsToTrack;
    private int _currentMaxOrbsToTrack;

    [SerializeField]
    private float _trackTime;

    [SerializeField]
    private bool _hasToLookAtOrb;

    [SerializeField]
    private SphereCollider _sphereCollider;

    [SerializeField]
    private bool _debug;


    private void Awake()
    {
        _trackedOrbs = new Dictionary<OrbMovementStateMachine, bool>();
        _currentMaxOrbsToTrack = _maxOrbsToTrack;
    }

    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != Tags.ORB_TAG || _trackedOrbs.Count == _currentMaxOrbsToTrack)
        {
            return;
        }

        OrbMovementStateMachine orb = other.transform.GetComponentInParent<OrbMovementStateMachine>();

        float dot = Vector3.Dot(this.transform.forward, orb.transform.forward);

        if ((dot < 0 || !_hasToLookAtOrb)) // pointing in opposite directions, meaning I am looking on the orb
        {
            // Orb is moving towards the player, slow it down
            AddOrb(orb);
        }
    }

    private IEnumerator StartTracking(OrbMovementStateMachine orbMovementStateMachine)
    {
        orbMovementStateMachine.Tracked(true);
        yield return new WaitForSeconds(_trackTime);
        orbMovementStateMachine.Tracked(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != Tags.ORB_TAG)
        {
            return;
        }

        OrbMovementStateMachine orb = other.transform.GetComponentInParent<OrbMovementStateMachine>();
        RemoveOrb(orb);
    }

    private void AddOrb(OrbMovementStateMachine orb)
    {
        if (!_trackedOrbs.ContainsKey(orb))
        {
            if (orb.CanBeTracked())
            {
                StartCoroutine(StartTracking(orb));
                _trackedOrbs.Add(orb, false);
            }
        }
    }

    private void RemoveOrb(OrbMovementStateMachine orb)
    {
        if (_trackedOrbs.ContainsKey(orb))
        {
            if (!_trackedOrbs[orb])
            {
                _trackedOrbs[orb] = true;
                StartCoroutine(StartOrbRemoval(orb));   
            } 
        }
    }

    private IEnumerator StartOrbRemoval(OrbMovementStateMachine orb)
    {
        yield return new WaitForSeconds(0.2f);
        _trackedOrbs.Remove(orb);
    }


    private void OnDrawGizmos()
    {

        if (!_debug) { return; }

        Gizmos.DrawWireSphere(this.transform.position, _sphereCollider.radius);

    }

}
