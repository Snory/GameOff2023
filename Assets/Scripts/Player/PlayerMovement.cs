using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{

    private Camera _mainCamera;
    private Vector2 _inputMoveVector;
    private Vector2 _inputRotateVector;

    private Rigidbody _rigidBody;

    [SerializeField]
    private float _movementSpeed;

    [SerializeField]
    private float _dashForce;
    [SerializeField]
    private float _dashDuration, _dashCooldown;

    [SerializeField]
    private bool _dashInForwardDirection;

    private bool _dashing, _canDash;

    private bool _canMove;

    [SerializeField]
    private LayerMask _walkableLayerMask;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rigidBody = GetComponent<Rigidbody>();

        _canDash = true;
        _dashing = false;
        _canMove = true;

    }
    private void LateUpdate()
    {

        if (!_canMove)
        {
            _rigidBody.velocity = Vector3.zero;
        }

        Dash();
        Move();

    }

    private void Dash()
    {
        if (!_dashing || !_canMove) { return; }

        if (_dashInForwardDirection)
        {
            _rigidBody.velocity = this.transform.forward * _dashForce;
        }
        else
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * _dashForce;
        }

    }

    private void Move()
    {
        if (_dashing || !_canMove) return;

        Vector3 direction = new Vector3(_inputMoveVector.x, 0, _inputMoveVector.y);
        _rigidBody.velocity = direction * _movementSpeed;
    }

    private void ResetDash()
    {
        _canDash = true;
    }

    private void StopDashing()
    {
        _dashing = false;
    }


    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed || !_canDash)
        {
            return;
        }

        _canDash = false;
        _dashing = true;

        Invoke("ResetDash", _dashCooldown);
        Invoke("StopDashing", _dashDuration);
    }



    /// <summary>
    /// This is going to move foward, backward and to sides (left joystick on gamepad)
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        _inputMoveVector = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// This is going to rotate the character (right joystick on the gamepad)
    /// </summary>
    /// <param name="context"></param>
    public void OnMouseRotate(InputAction.CallbackContext context)
    {

        _inputRotateVector = context.ReadValue<Vector2>();

        // Cast a ray from the camera to the mouse position
        Ray ray = _mainCamera.ScreenPointToRay(_inputRotateVector);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _walkableLayerMask))
        {

            // Calculate the direction to the mouse pointer
            Vector3 targetDirection = hit.point - transform.position;

            targetDirection.y = 0;


            // Rotate the object towards the mouse
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _rigidBody.MoveRotation(targetRotation);

        }
    }

    public void OnGamePadRotate(InputAction.CallbackContext context)
    {
        _inputRotateVector = context.ReadValue<Vector2>();

        if (_inputRotateVector == Vector2.zero) return;

        Vector3 rotate = new Vector3(_inputRotateVector.x, 0, _inputRotateVector.y);

        Quaternion targetRotation = Quaternion.LookRotation(rotate);
        _rigidBody.MoveRotation(targetRotation);

    }

    public void OnCatched(bool catched)
    {
        _canMove = !catched;
    }
}
