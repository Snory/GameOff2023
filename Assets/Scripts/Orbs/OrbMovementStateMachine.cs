using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum OrbMovementStates { CATCHED, BOUCING_MOVEMENT, TRACKED, RELEASED, RELEASED_CONTINUE}

public class OrbMovementStateMachine : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField]
    public OrbMovementStates _currentState;

    [SerializeField]
    private OrbMovement _currentOrbMovement;

    private OrbBouncingMovement _bouncingMovement;
    private OrbTrackedMovement _trackedMovement;

    [SerializeField]
    private CachedComponents _cachedComponents;

    [SerializeField]
    private float _trackedCoolDown, _currentTrackTime;

    [SerializeField]
    private float _releasedCoolDown, _currentReleasedTime;

    private void Awake()
    {
        _currentTrackTime = _trackedCoolDown;
    }

    private void Update()
    {
        WatchTrackTime();
        WatchReleaseTime();
    }

    private void WatchReleaseTime()
    {
        _currentReleasedTime += Time.deltaTime;

        if(_currentReleasedTime > _releasedCoolDown)
        {
            if (_currentState == OrbMovementStates.RELEASED || _currentState == OrbMovementStates.RELEASED_CONTINUE)
            {
                TransitToState(OrbMovementStates.BOUCING_MOVEMENT);
            }
        }

    }

    private void WatchTrackTime()
    {
        _currentTrackTime += Time.deltaTime;
    }

    private void Start()
    {
        _rigidbody = _cachedComponents.GetComponentFromRoot<Rigidbody>();
        _bouncingMovement = _cachedComponents.GetComponentFromRoot<OrbBouncingMovement>();
        _trackedMovement = _cachedComponents.GetComponentFromRoot<OrbTrackedMovement>();
        TransitToState(_currentState);
    }

    
    private void TransitToState(OrbMovementStates newState)
    {
        if(_currentOrbMovement is not null)
        {
            _currentOrbMovement.OnExit(_cachedComponents);
        }

        _currentState = newState;

        switch(_currentState)
        {
            case OrbMovementStates.BOUCING_MOVEMENT:
                _currentOrbMovement = _bouncingMovement;
                break;
            case OrbMovementStates.TRACKED:
                _currentOrbMovement = _trackedMovement;
                _currentTrackTime = 0;
                break;
            case OrbMovementStates.CATCHED:
                _currentOrbMovement = null;                
                break;
            case OrbMovementStates.RELEASED:
                _currentOrbMovement = _bouncingMovement;
                _currentReleasedTime = 0;
                break;
            case OrbMovementStates.RELEASED_CONTINUE:
                _currentOrbMovement = _bouncingMovement;
                break;
        }

        if(_currentOrbMovement is not null)
        {
            _currentOrbMovement.OnEnter(_cachedComponents);
        }
    }

    private void LateUpdate()
    {
        if (_rigidbody.velocity != Vector3.zero)
        {
            // Calculate the rotation to look in the direction of velocity
            Quaternion targetRotation = Quaternion.LookRotation(_rigidbody.velocity, Vector3.up);

            // Apply the rotation to the Rigidbody's transform
            _rigidbody.MoveRotation(targetRotation);
        }
    }

    public void Tracked(bool tracked)
    {
        if (tracked)
        {
            TransitToState(OrbMovementStates.TRACKED);
        } else if (_currentState == OrbMovementStates.TRACKED)
        {
            if (Released())
            {
                TransitToState(OrbMovementStates.RELEASED_CONTINUE);

            } else
            {
                TransitToState(OrbMovementStates.BOUCING_MOVEMENT);
            }
        }
    }

    public void Catch()
    {
        TransitToState(OrbMovementStates.CATCHED);
    }

    public bool Catched()
    {
        return _currentState == OrbMovementStates.CATCHED;
    }

    public bool CanBeTracked()
    {
        return (_currentTrackTime >= _trackedCoolDown);
    }

    public bool Tracked()
    {
        return _currentState == OrbMovementStates.TRACKED;
    }

    public bool Released()
    {
        return (_currentReleasedTime < _releasedCoolDown);
    }

    internal void Redirect(Vector3 releasePosition, Vector3 forward)
    {
        this.transform.position = releasePosition;
        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
        this.transform.rotation = targetRotation;
        TransitToState(OrbMovementStates.RELEASED);
    }
}
