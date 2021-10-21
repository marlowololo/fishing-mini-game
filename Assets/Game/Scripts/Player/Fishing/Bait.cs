using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour {
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] GameObject _baitMesh;

    public Fish Fish;

    public delegate void OnBaitTakenDelegate();
    private OnBaitTakenDelegate _onBaitTaken;
    public delegate void OnFishLostDelegate();
    private OnFishLostDelegate _onFishLost;
    public delegate void OnBaitPulledDelegate();
    private OnBaitPulledDelegate _onBaitPulled;

    public bool IsBaitTaken = false;
    private bool _contactWithWater = false;
    private float _inWaterTime;
    private Vector3 _throwDirection;

    public void Init(OnBaitTakenDelegate onBaitTaken, OnFishLostDelegate onFishLost) {
        _onBaitTaken = onBaitTaken;
        _onFishLost = onFishLost;
    }

    private void Update() {
        if (_contactWithWater) {
            _inWaterTime += Time.deltaTime;
            _baitMesh.transform.localPosition = (Vector3.up * Mathf.Sin(_inWaterTime)) + Vector3.up;
        }
    }

    public void AddOnBaitPulledCallback(OnBaitPulledDelegate onBaitPulled) {
        _onBaitPulled += onBaitPulled;
    }

    public void CastBait(Vector3 startPosition, Vector3 direction, float throwForce) {
        _onBaitPulled = null;
        gameObject.SetActive(true);
        _rigidBody.velocity = Vector3.zero;
        transform.position = startPosition;
        _rigidBody.AddForce(throwForce * direction);
        IsBaitTaken = false;
        _contactWithWater = false;
        _inWaterTime = 0;
        _throwDirection = direction;
    }

    public void OnBaitTaken() {
        IsBaitTaken = true;
        gameObject.SetActive(false);
        _contactWithWater = false;
        _onBaitTaken?.Invoke();
    }

    public void OnFishLost(){
        Fish = null;
        _onFishLost?.Invoke();
    }

    public void Pull() {
        gameObject.SetActive(false);
        _contactWithWater = false;
        _onBaitPulled?.Invoke();
        _onBaitPulled = null;
    }

    public Transform GetBaitMeshTransform() {
        return _baitMesh.transform;
    }

    private void OnCollisionEnter(Collision other) {
        _rigidBody.velocity = Vector3.zero;
        if (other.collider.CompareTag("Water")) {
            _contactWithWater = true;
        }
    }

    public Vector3 GetThrowDirection() {
        return _throwDirection;
    }
}
