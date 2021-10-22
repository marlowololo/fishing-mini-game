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
    [SerializeField] private PlayerCameraController _playerCamera;
    [SerializeField] private FishingLine _fishingLine;
    [SerializeField] private AudioSource _holdCastAudioSource;
    [SerializeField] private AudioSource _fishingRodAudioSource;
    [SerializeField] private AudioSource _newItemAudioSource;


    [Header("Locations References")]
    [SerializeField] private Transform _fishingMeterUIRef;
    [SerializeField] private GameObject _raycastOriginRef;
    [SerializeField] private GameObject _baitCastPositionRef;
    [SerializeField] private GameObject _fishCatchPositionRef;

    [Header("Prefabs")]
    [SerializeField] private Bait _baitPrefabs;

    //animator state ID
    private int _isMovingStateID;
    private int _castRodStateID;
    private int _throwRodStateID;
    private int _pullFishStateID;
    private int _baitTakenStateID;
    private int _catchFishID;

    //movement
    private Vector3 _movementVector = Vector3.zero;
    private bool _isMoving;

    //fishing
    private bool _isCastingFishingRod;
    private bool _castThrown;
    private bool _fishingRodPulled;
    private bool _isFishing;
    private bool _lerpFishPosition;
    private float _fishingCastHoldTime;
    private Bait _bait;

    //fishing catch lerp
    private Quaternion _fishRotation = Quaternion.Euler(0, 90, 0);
    private Vector3 _fishFirstPosition;
    private float _fishCatchLerpTime;

    private void Start() {
        //animation id
        _isMovingStateID = Animator.StringToHash("IsMoving");
        _castRodStateID = Animator.StringToHash("CastRod");
        _throwRodStateID = Animator.StringToHash("ThrowRod");
        _pullFishStateID = Animator.StringToHash("PullFish");
        _baitTakenStateID = Animator.StringToHash("BaitTaken");
        _catchFishID = Animator.StringToHash("CatchFish");

        //object instance
        _bait = Instantiate<Bait>(_baitPrefabs);
        _bait.Init(OnBaitTaken, PullFishingRod);
        _bait.gameObject.SetActive(false);

        //fishing state
        _castThrown = false;
        _fishingRodPulled = false;
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

        if (_lerpFishPosition) {
            LerpFishPositionOnCatch();
        }
    }

    private void TryCastFishing() {
        RaycastHit raycastHit;
        if (Physics.Raycast(_raycastOriginRef.transform.position, Vector3.down, out raycastHit, 10, 1 << 4)) {
            if (raycastHit.collider.CompareTag("Water")) {
                StopAllCoroutines();

                _fishingCastHoldTime = 0;
                _isCastingFishingRod = true;
                _isFishing = false;
                _castThrown = false;
                _lerpFishPosition = false;

                _holdCastAudioSource.Play();

                _playerAnimator.SetBool(_pullFishStateID, false);
                _playerAnimator.SetBool(_catchFishID, false);
                _playerAnimator.SetBool(_castRodStateID, true);
                _playerUIController.ShowFishingCastingMeter(_fishingMeterUIRef.position);
                _playerUIController.SetFishingCastingMeterValue(0);
            }
        }
    }

    private void CastFishingRod() {
        _fishingCastHoldTime += Time.deltaTime;
        if (_fishingCastHoldTime > _fishingCastHoldTimeLimit) {
            _fishingCastHoldTime = _fishingCastHoldTimeLimit;
        }

        _holdCastAudioSource.pitch = 0.5f + (_fishingCastHoldTime / _fishingCastHoldTimeLimit);
        _playerUIController.SetFishingCastingMeterValue(_fishingCastHoldTime / _fishingCastHoldTimeLimit);

        if (Input.GetButtonUp("Fire1") && !_castThrown) {
            _castThrown = true;
            _fishingRodPulled = false;
            _fishingRodAudioSource.Play();
            _holdCastAudioSource.Stop();


            _playerUIController.HideFishingCastingMeter();

            _playerAnimator.SetBool(_castRodStateID, false);
            _playerAnimator.SetBool(_throwRodStateID, true);

            float throwPower = _minimumFishingPower + (_fishingCastHoldTime / _fishingCastHoldTimeLimit * (_maximumFishingPower - _minimumFishingPower));

            StartCoroutine(
                WaitBaitCasting(throwPower)
            );

            _playerCamera.EnabeFishingCamera(_playerMesh.transform.forward.normalized);
        }

        IEnumerator WaitBaitCasting(float fishingThrowPower) {
            yield return new WaitForSeconds(0.25f);
            _bait.CastBait(
                _baitCastPositionRef.transform.position,
                _playerMesh.transform.forward,
                fishingThrowPower
            );
            _fishingLine.ShowLine(_baitCastPositionRef.transform, _bait.GetBaitMeshTransform());
            yield return new WaitForSeconds(0.5f);
            _isCastingFishingRod = false;
            _isFishing = true;
        }
    }

    private void Fishing() {
        if (Input.GetButtonDown("Fire1") && !_fishingRodPulled) {
            PullFishingRod();
        }
    }

    private void OnBaitTaken() {
        _playerAnimator.SetBool(_baitTakenStateID, true);
        _playerUIController.OnBaitTaken(_raycastOriginRef.transform.position);
        _fishingLine.ShowLine(_baitCastPositionRef.transform, _bait.Fish.GetHookPosition());
    }

    private void PullFishingRod() {
        _fishingRodPulled = true;

        _playerAnimator.SetBool(_throwRodStateID, false);
        _playerAnimator.SetBool(_baitTakenStateID, false);
        _playerAnimator.SetBool(_pullFishStateID, true);

        _playerUIController.HideBaitIndicator();

        _playerCamera.DisableFishingCamera();

        if (_bait.IsBaitTaken && _bait.Fish != null) {
            PullFish();
        } else {
            _bait.Pull();
            _fishingLine.HideLine();
            _fishingRodAudioSource.Play();
            StartCoroutine(WaitForPullAnimation());
        }

        IEnumerator WaitForPullAnimation() {
            yield return new WaitForSeconds(1.25f);
            _isFishing = false;
        }
    }

    private void PullFish() {
        //pardon the hardcoded
        float defaultFishDistance = 3.971839f; //calculated manually
        float defaultPullForce = 150;

        _bait.Fish.GetCaught();

        Rigidbody fish = _bait.Fish.GetComponent<Rigidbody>();
        Vector3 pullDirection = (transform.position - _bait.Fish.transform.position).normalized;
        fish.AddForce(Vector3.up * 350);

        float fishDistance = Vector3.Distance(fish.transform.position, transform.position);
        float finalForcePower = defaultPullForce * fishDistance / defaultFishDistance;
        fish.AddForce(pullDirection * finalForcePower);

        _playerAnimator.SetBool(_catchFishID, true);

        StartCoroutine(CatchAnimationSequence());

        IEnumerator CatchAnimationSequence() {
            yield return new WaitForSeconds(1f);
            _lerpFishPosition = true;
            fish.velocity = Vector3.zero;
            fish.useGravity = false;
            fish.constraints = RigidbodyConstraints.FreezePosition;

            _fishFirstPosition = fish.transform.position;
            _fishCatchLerpTime = 0;

            yield return new WaitForSeconds(1.0f);
            _isFishing = false;
            _lerpFishPosition = false;
            _fishingLine.HideLine();
            _newItemAudioSource.Play();
            Destroy(fish.gameObject);
        }
    }

    private void LerpFishPositionOnCatch() {
        if(_bait.Fish == null){
            _lerpFishPosition = false;
            return;
        }
        _fishCatchLerpTime += Time.deltaTime;
        Transform fishTransform = _bait.Fish.transform;
        fishTransform.position = Vector3.Lerp(_fishFirstPosition, _fishCatchPositionRef.transform.position, _fishCatchLerpTime / 0.25f);
        fishTransform.rotation = _playerMesh.transform.rotation * Quaternion.Lerp(fishTransform.rotation, _fishRotation, _fishCatchLerpTime / 0.25f);
    }

}
