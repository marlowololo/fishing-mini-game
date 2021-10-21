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
    private bool _startChasing = false;
    private float _currentChaseTime = 0f;

    private void Update() {
        if (_startChasing) {
            _currentChaseTime += Time.deltaTime;
            transform.position = Vector3.Lerp(_startingPosition, _targetPosition, _currentChaseTime / _chasingTime);
            if (_currentChaseTime >= _chasingTime) {
                _startChasing = false;
                _bait.OnBaitTaken();
            }
        }
    }

    public void ChaseBait(Bait bait) {
        _fishAnimator.speed = 1;
        _bait = bait;
        _bait.Fish = this;
        _startingPosition = transform.position;
        _targetPosition = _bait.transform.position;
        _targetPosition.y = _startingPosition.y;
        _startChasing = true;
        _currentChaseTime = 0f;
        transform.LookAt(_bait.transform.position);
    }

    public void GetCaught() {
        _fishAnimator.speed = 10;
    }

    public void OnBaitPulled() {
        Destroy(gameObject);
    }

    public Transform GetHookPosition(){
        return _fishHookPosRef;
    }
}
