using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBouncingMovement : OrbMovement
{

    [SerializeField]
    private float _initialSpeed;

    [SerializeField]
    private float _maxSpeed, _minSpeed;

    private float _currentSpeed;

    [SerializeField]
    private float _speedMultiplier;

    private Rigidbody _rigidbody;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 reflection = Vector3.Reflect(this.transform.forward, normal);

        float speed = _currentSpeed + _currentSpeed * _speedMultiplier;
        SetVelocity(reflection.normalized , speed);
    }

    private void Update()
    {
        if(_rigidbody.velocity == Vector3.zero)
        {
            SetVelocity(this.transform.forward, _currentSpeed);
        }
    }

    public override void SetVelocity(Vector3 direction, float speed)
    {
        _currentSpeed = speed;
        if (_currentSpeed >= _maxSpeed)
        {
            _currentSpeed = _maxSpeed;
        } else if( _currentSpeed < _minSpeed)
        {
            _currentSpeed = _minSpeed;
        }

        _rigidbody.velocity = direction * _currentSpeed;
    }

    public override void OnEnter(CachedComponents cached)
    {
        _rigidbody = cached.GetComponentFromRoot<Rigidbody>(); 
        _currentSpeed = _initialSpeed;
        SetVelocity(this.transform.forward, _currentSpeed);
    }

    public override void OnExit(CachedComponents cached)
    {
        _initialSpeed = _currentSpeed;
        SetVelocity(Vector3.zero, 0);
    }
}
