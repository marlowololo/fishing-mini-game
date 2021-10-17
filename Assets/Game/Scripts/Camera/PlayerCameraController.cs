using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
    
    [Header("References")]
    [SerializeField] private GameObject _player;

    private Vector3 _positionOffset;

    private void Start() {
        _positionOffset = transform.position;
    }


    private void Update() {
        transform.position = _player.transform.position + _positionOffset;
    }
}
