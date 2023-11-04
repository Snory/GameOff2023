using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTrackedMovement : OrbMovement
{
    [SerializeField]
    private float _trackedSpeed;

    private Rigidbody _rigidbody;

    public override void SetVelocity(Vector3 direction, float speed)
    {
        _rigidbody.velocity = direction * _trackedSpeed;
    }

    public override void OnEnter(CachedComponents cached)
    {
        _rigidbody = cached.GetComponentFromRoot<Rigidbody>();
        SetVelocity(this.transform.forward, _trackedSpeed);
    }

    public override void OnExit(CachedComponents cached)
    {
        SetVelocity(Vector3.zero, 0);
    }

}
