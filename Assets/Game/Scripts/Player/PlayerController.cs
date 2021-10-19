using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _fishingCastHoldTimeLimit = 2.5f;
    [SerializeField] private float _minimumFishingPower = 60f;
    [SerializeField] private float _maximumFishingPower = 260f;

    [Header("Object References")]
    [SerializeField] private PlayerUIController _playerUIController;
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Rigidbody _playerRigidbody;

    [Header("Locatios References")]
    [SerializeField] private Transform _fishingMeterUIRef;
    [SerializeField] private GameObject _raycastOriginRef;
    [SerializeField] private GameObject _baitCastPositionRef;

    [Header("Prefabs")]
    [SerializeField] private Bait _baitPrefabs;

    //animator state ID
    private int _isMovingStateID;
    private int _castRodStateID;
    private int _throwRodStateID;
    private int _pullFishStateID;
    private int _baitTakenStateID;

    //movement
    private Vector3 _movementVector = Vector3.zero;
    private bool _isMoving;

    //fishing
    private bool _isCastingFishingRod;
    private bool _castThrown;
    private bool _isFishing;
    private float _fishingCastHoldTime;
    private Bait _bait;

    private void Start() {
        //animation id
        _isMovingStateID = Animator.StringToHash("IsMoving");
        _castRodStateID = Animator.StringToHash("CastRod");
        _throwRodStateID = Animator.StringToHash("ThrowRod");
        _pullFishStateID = Animator.StringToHash("PullFish");
        _baitTakenStateID = Animator.StringToHash("BaitTaken");

        //object instance
        _bait = Instantiate<Bait>(_baitPrefabs);
        _bait.Init(OnBaitTaken);
        _bait.gameObject.SetActive(false);

        //fishing state
        _castThrown = false;
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

        if (!_isCastingFishingRod && !_isFishing) {
            //movement
            float xInput = Input.GetAxisRaw("Horizontal");
            float yInput = Input.GetAxisRaw("Vertical");
            _movementVector.Set(xInput, 0, yInput);
            _movementVector = _movementVector.normalized;
            _isMoving = xInput != 0 || yInput != 0;
            _playerAnimator.SetBool(_isMovingStateID, _isMoving);

            //casting rod
            if (Input.GetButtonDown("Fire1") && !_isMoving) {
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
        Debug.Log("CASTING WA!");
        RaycastHit raycastHit;
        if (Physics.Raycast(_raycastOriginRef.transform.position, Vector3.down, out raycastHit, 3, 1 << 4)) {
            if (raycastHit.collider.CompareTag("Water")) {
                StopAllCoroutines();

                _fishingCastHoldTime = 0;
                _isCastingFishingRod = true;
                _isFishing = false;
                _castThrown = false;

                _playerAnimator.SetBool(_pullFishStateID, false);
                _playerAnimator.SetBool(_castRodStateID, true);
                _playerUIController.ShowFishingCastingMeter(_fishingMeterUIRef.position);
                _playerUIController.SetFishingCastingMeterValue(0);
            }
        }
    }

    private void CastFishingRod() {
        _fishingCastHoldTime += Time.deltaTime;
        _playerUIController.SetFishingCastingMeterValue(_fishingCastHoldTime / _fishingCastHoldTimeLimit);
        if (Input.GetButtonUp("Fire1") && !_castThrown) {
            _castThrown = true;

            if (_fishingCastHoldTime > _fishingCastHoldTimeLimit) {
                _fishingCastHoldTime = _fishingCastHoldTimeLimit;
            }

            _playerUIController.HideFishingCastingMeter();

            _playerAnimator.SetBool(_castRodStateID, false);
            _playerAnimator.SetBool(_throwRodStateID, true);

            float throwPower = _minimumFishingPower + (_fishingCastHoldTime / _fishingCastHoldTimeLimit * (_maximumFishingPower - _minimumFishingPower));

            StartCoroutine(
                WaitBaitCasting(throwPower)
            );
        }

        IEnumerator WaitBaitCasting(float fishingThrowPower) {
            yield return new WaitForSeconds(0.25f);
            _bait.CastBait(
                _baitCastPositionRef.transform.position,
                _playerMesh.transform.forward,
                fishingThrowPower
            );
            yield return new WaitForSeconds(0.5f);
            _isCastingFishingRod = false;
            _isFishing = true;
        }
    }

    private void Fishing() {
        if (Input.GetButtonDown("Fire1")) {
            _playerAnimator.SetBool(_throwRodStateID, false);
            _playerAnimator.SetBool(_baitTakenStateID, false);
            _playerAnimator.SetBool(_pullFishStateID, true);

            StartCoroutine(WaitForFishingAnimation());
            _playerUIController.HideBaitIndicator();
            if (_bait.IsBaitTaken) {
                PullFish();
            } else {
                _bait.Pull();
            }
        }

        IEnumerator WaitForFishingAnimation() {
            yield return new WaitForSeconds(1.55f);
            _isFishing = false;
        }

    }

    private void OnBaitTaken() {
        _playerAnimator.SetBool(_baitTakenStateID, true);
        _playerUIController.OnBaitTaken(_raycastOriginRef.transform.position);
    }

    private void PullFish() {
        var fish = _bait.Fish.GetComponent<Rigidbody>();
        Vector3 pullDirection = (transform.position - _bait.Fish.transform.position).normalized;
        fish.AddForce(Vector3.up * 350);
        fish.AddForce(pullDirection * 150);
        //maxheight = 2.45f
    }

}
