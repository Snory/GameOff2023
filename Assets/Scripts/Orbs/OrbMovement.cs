using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrbMovement : MonoBehaviour
{

    public abstract void OnEnter(CachedComponents cached);

    public abstract void OnExit(CachedComponents cached);

    public abstract void SetVelocity(Vector3 direction, float speed);

}
