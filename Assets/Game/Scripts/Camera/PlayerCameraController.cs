using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField, Range(0,1)] private float _cameraSmoothness = 0.2f;

    private Vector3 _positionOffset;

    private void Start() {
        _positionOffset = transform.position;
    }

    private void FixedUpdate() {
        Vector3 targetPosition = _player.transform.position + _positionOffset;
        Vector3 smoothedTargerPosition = Vector3.Lerp(transform.position, targetPosition, _cameraSmoothness);
        transform.position = smoothedTargerPosition;
    }
}
