using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject _player;

    [Header("Settings")]
    [SerializeField, Range(0,1)] private float _cameraSmoothness = 0.2f;
    [SerializeField] private float _fishingCameraXOffset;
    [SerializeField] private float _fishingCameraYOffset;


    private Vector3 _positionOffset;
    private Vector3 _fishingCameraOffset;
    private Vector3 _fishingCameraDirection;
    private bool _useFishingCamera;

    private void Start() {
        _positionOffset = transform.position;
        _fishingCameraOffset = Vector3.zero;
        _useFishingCamera = false;
    }

    private void FixedUpdate() {
        if(_useFishingCamera){
            _fishingCameraOffset = _fishingCameraDirection * _fishingCameraXOffset;
            _fishingCameraOffset.y += _fishingCameraYOffset;
        } else {
            _fishingCameraOffset = Vector3.zero;
        }
        Vector3 targetPosition = _player.transform.position + _positionOffset + _fishingCameraOffset;
        Vector3 smoothedTargetPosition = Vector3.Lerp(transform.position, targetPosition, _cameraSmoothness);
        transform.position = smoothedTargetPosition;
    }

    public void EnabeFishingCamera(Vector3 direction){
        _useFishingCamera = true;
        _fishingCameraDirection = direction;
    }

    public void DisableFishingCamera(){
        _useFishingCamera = false;
    }
}
