using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Animator _fishAnimator;
    [SerializeField] private Transform _fishHookPosRef;

    [Header("Settings")]
    [SerializeField] private float _chasingTime = 3f;

    private Bait _bait;

    private Vector3 _startingPosition;
    private Vector3 _targetPosition;
    private Vector3 _baitThrowDirection;

    private bool _startChasing = false;
    private bool _strike = false;
    private float _currentChaseTime = 0f;
    private float _swimTime = 0f;
    private float _swimTimeLimit = 2f;
    private int _swimChangeCount = 0;
    private int _swimChangeCountLimit = 3;

    private void Update() {
        if (_startChasing) {
            _currentChaseTime += Time.deltaTime;
            transform.position = Vector3.Lerp(_startingPosition, _targetPosition, _currentChaseTime / _chasingTime);
            if (_currentChaseTime >= _chasingTime) {
                _startChasing = false;
                _strike = true;
                _bait.OnBaitTaken();
                ChangeSwimDirection();
            }
        }

        if (_strike) {
            _swimTime += Time.deltaTime;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.3f, Space.Self);
            if (_swimTime >= _swimTimeLimit) {
                _swimTime = 0;
                _swimChangeCount++;
                ChangeSwimDirection();
                if(_swimChangeCount >= _swimChangeCountLimit){
                    _strike = false;
                    _bait.OnFishLost();
                }
            }
        }
    }

    private void ChangeSwimDirection() {
        transform.LookAt(transform.position + _baitThrowDirection);
        transform.Rotate(0f, Random.Range(-30f, 30f), 0f);
    }

    public void ChaseBait(Bait bait) {
        _fishAnimator.speed = 1;
        _bait = bait;
        _bait.Fish = this;
        _startingPosition = transform.position;
        _targetPosition = _bait.transform.position;
        _targetPosition.y = _startingPosition.y;
        _startChasing = true;
        _strike = false;
        _currentChaseTime = 0f;
        transform.LookAt(_bait.transform.position);

        _swimChangeCount = 0;
        _baitThrowDirection = bait.GetThrowDirection();
        _baitThrowDirection.y = transform.position.y;
    }

    public void GetCaught() {
        _fishAnimator.speed = 10;
    }

    public void OnBaitPulled() {
        Destroy(gameObject);
    }

    public Transform GetHookPosition() {
        return _fishHookPosRef;
    }
}
