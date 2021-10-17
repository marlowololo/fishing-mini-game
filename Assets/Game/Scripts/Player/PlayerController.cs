using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 10f;

    [Header("References")]
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private Animator _playerAnimator;

    private int _isMovingStateID;
    private Vector3 _movementVector = Vector3.zero;

    private void Start() {
        _isMovingStateID = Animator.StringToHash("IsMoving");
    }

    private void Update() {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        _movementVector.Set(xInput, 0, yInput);
        bool isMoving = xInput != 0 || yInput != 0;
        _playerAnimator.SetBool(_isMovingStateID, isMoving);
        if (isMoving) {
            _playerMesh.transform.rotation = Quaternion.LookRotation(_movementVector);
            transform.Translate(_movementVector * Time.deltaTime * _movementSpeed);
        }
    }
}
