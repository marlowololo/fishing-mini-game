using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _fishingCastHoldTimeLimit = 2.5f;

    [Header("References")]
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private GameObject _raycastOriginRef;
    [SerializeField] private PlayerUIController _playerUIController;

    //animator state ID
    private int _isMovingStateID;
    private int _castRodStateID;
    private int _throwRodStateID;
    private int _pullFishStateID;

    //movement
    private Vector3 _movementVector = Vector3.zero;
    private bool _isMoving;

    //fishing
    private bool _isCastingFishingRod;
    private bool _isFishing;
    private float _fishingCastHoldTime;

    private void Start() {
        _isMovingStateID = Animator.StringToHash("IsMoving");
        _castRodStateID = Animator.StringToHash("CastRod");
        _throwRodStateID = Animator.StringToHash("ThrowRod");
        _pullFishStateID = Animator.StringToHash("PullFish");

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
        if (!_isFishing && !_isCastingFishingRod) {
            float xInput = Input.GetAxisRaw("Horizontal");
            float yInput = Input.GetAxisRaw("Vertical");
            _movementVector.Set(xInput, 0, yInput);
            _movementVector = _movementVector.normalized;
            _isMoving = xInput != 0 || yInput != 0;
            _playerAnimator.SetBool(_isMovingStateID, _isMoving);
        }

        //casting
        if (!_isCastingFishingRod && !_isFishing) {
            if (Input.GetButtonDown("Fire1")) {
                TryCastFishing();
            }
        }

        if (_isCastingFishingRod) {
            CastFishingRod();
        }

        if (_isFishing) {
            Fishing();
        }
    }

    private void TryCastFishing() {
        RaycastHit raycastHit;
        if (Physics.Raycast(_raycastOriginRef.transform.position, Vector3.down, out raycastHit, 3)) {
            if (raycastHit.collider.CompareTag("Water")) {
                _fishingCastHoldTime = 0;
                _isCastingFishingRod = true;
                _playerAnimator.SetTrigger(_castRodStateID);
                _playerUIController.ShowFishingCastingMeter();
                _playerUIController.SetFishingCastingMeterValue(0);
            }
        }
    }

    private void CastFishingRod() {
        _fishingCastHoldTime += Time.deltaTime;
        _playerUIController.SetFishingCastingMeterValue(_fishingCastHoldTime / _fishingCastHoldTimeLimit);
        if (Input.GetButtonUp("Fire1")) {
            _playerUIController.HideFishingCastingMeter();
            _playerAnimator.SetTrigger(_throwRodStateID);
            _isCastingFishingRod = false;
            _isFishing = true;
        }
    }

    private void Fishing() {
        if (Input.GetButtonDown("Fire1")) {
            _playerAnimator.SetTrigger(_pullFishStateID);
            _isFishing = false;
        }
    }

}
