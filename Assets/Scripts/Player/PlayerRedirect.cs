using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;



public class PlayerRedirect : MonoBehaviour
{
    [SerializeField]
    private bool _debug;

    [SerializeField]
    private float _castRadius;

    [SerializeField]
    private LayerMask _orbLayerMask;

    [SerializeField]
    private OrbMovementStateMachine _selectedOrb;

    public UnityEvent<bool> Catched;

    [SerializeField]
    private Transform _releaseSpot;

    [SerializeField]
    private List<OrbMovementStateMachine> _orbsCandidates;


    private void Awake()
    {
        _orbsCandidates = new List<OrbMovementStateMachine>();
    }

    public void OnRedirect(InputAction.CallbackContext context)
    {
        if (!context.performed || _selectedOrb is null) { return; }

        if (!_selectedOrb.Catched())
        {
            Debug.Log("Catch");

            _selectedOrb.Catch();
            Catched.Invoke(true);
        }
        else
        {
            Debug.Log("Release");

            _selectedOrb.Redirect(_releaseSpot.position, this.transform.forward);
            _selectedOrb = GetClosestCandidate();
            Catched.Invoke(false);
        }
    }

    public void OnSelectedTrackObject(OrbMovementStateMachine orbMovementStateMachine)
    {
        if (!_orbsCandidates.Contains(orbMovementStateMachine))
        {
            _orbsCandidates.Add(orbMovementStateMachine);
        }

        if (_selectedOrb is not null && _selectedOrb.Catched()) { return; }

        _selectedOrb = GetClosestCandidate();
    }

    public void OnDeselectedTrackObject(OrbMovementStateMachine orbMovementStateMachine)
    {
        if (_orbsCandidates.Contains(orbMovementStateMachine))
        {
            _orbsCandidates.Remove(orbMovementStateMachine);
        }

        if (_selectedOrb is not null && _selectedOrb.Catched()) { return; }

        if(_selectedOrb == orbMovementStateMachine)
        {
            _selectedOrb = GetClosestCandidate();
        }
    }

    private OrbMovementStateMachine GetClosestCandidate()
    {
        OrbMovementStateMachine candidate = null;

        if(_orbsCandidates.Count > 0)
        {
            candidate = _orbsCandidates[0];
            float distance = Vector3.Distance(this.transform.position, candidate.transform.position);
            for (int i = 1; i < _orbsCandidates.Count; i++)
            {
                if(distance > Vector3.Distance(this.transform.position, candidate.transform.position))
                {
                    candidate = _orbsCandidates[i];
                }
            }
        }

        return candidate;
    }


    private void OnDrawGizmos()
    {
        if (!_debug) { return; }

        Gizmos.DrawWireSphere(this.transform.position + this.transform.forward * 5, _castRadius);
    }

}
