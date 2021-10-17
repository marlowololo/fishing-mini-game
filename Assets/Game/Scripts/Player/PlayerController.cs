using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 10f;

    [Header("References")]
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private GameObject _raycastOriginRef;

    //animator state ID
    private int _isMovingStateID;
    private int _castRodStateID;

    private Vector3 _movementVector = Vector3.zero;
    private bool _isMoving;
    private bool _isFishing;

    private void Start() {
        _isMovingStateID = Animator.StringToHash("IsMoving");
        _castRodStateID = Animator.StringToHash("CastRod");
    }

    private void Update() {
        PlayerInput();
    }

    private void FixedUpdate() {
        if (_isMoving) {
            _playerMesh.transform.rotation = Quaternion.LookRotation(_movementVector);
            _playerRigidbody.velocity = _movementVector * _movementSpeed;
        } else {
            _playerRigidbody.velocity = Vector3.zero;
        }
    }

    private void PlayerInput() {
        //movement
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        _movementVector.Set(xInput, 0, yInput);
        _movementVector = _movementVector.normalized;
        _isMoving = xInput != 0 || yInput != 0;
        _playerAnimator.SetBool(_isMovingStateID, _isMoving);

        //casting
        if (!_isFishing) {
            if (Input.GetButtonDown("Fire1")) {
                TryCastFishing();
            }
        }
    }

    private void TryCastFishing() {
        RaycastHit raycastHit;
        if (Physics.Raycast(_raycastOriginRef.transform.position, Vector3.down, out raycastHit, 3)) {
            if (raycastHit.collider.CompareTag("Water")) {
                _isFishing = true;
                _playerAnimator.SetTrigger(_castRodStateID);
            }
        }
    }
}
