using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

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
        _bait = bait;
        _bait.Fish = this;
        _startingPosition = transform.position;
        _targetPosition = _bait.transform.position;
        _targetPosition.y = _startingPosition.y;
        _startChasing = true;
        _currentChaseTime = 0f;
        transform.LookAt(_bait.transform.position);
    }

    public void OnBaitPulled(){
        Destroy(gameObject);
    }
}
