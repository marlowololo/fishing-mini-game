using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour {
    [SerializeField] Rigidbody _rigidBody;

    public Fish Fish;

    public delegate void OnBaitTakenDelegate();
    private OnBaitTakenDelegate _onBaitTaken;
    public delegate void OnBaitPulledDelegate();
    private OnBaitPulledDelegate _onBaitPulled;

    public bool IsBaitTaken = false;

    public void Init(OnBaitTakenDelegate onBaitTaken) {
        _onBaitTaken = onBaitTaken;
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
    }

    public void OnBaitTaken() {
        IsBaitTaken = true;
        gameObject.SetActive(false);
        _onBaitTaken?.Invoke();
    }

    public void Pull() {
        gameObject.SetActive(false);
        _onBaitPulled?.Invoke();
        _onBaitPulled = null;
    }

    private void OnCollisionEnter(Collision other) {
        _rigidBody.velocity = Vector3.zero;
    }
}
