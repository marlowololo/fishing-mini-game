using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour {
    [SerializeField] private LineRenderer _lineRenderer;

    private bool _showLine;
    private Transform _from, _to;

    private void Start() {
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
    }

    private void Update() {
        if (_showLine) {
            _lineRenderer.SetPosition(0, _from.position);
            _lineRenderer.SetPosition(1, _to.position);
        }
    }

    public void ShowLine(Transform from, Transform to) {
        _lineRenderer.enabled = true;
        _showLine = true;
        _from = from;
        _to = to;
        _lineRenderer.SetPosition(0, _from.position);
        _lineRenderer.SetPosition(1, _to.position);
    }

    public void HideLine() {
        _lineRenderer.enabled = false;
        _showLine = false;
        _from = null;
        _to = null;
    }
}
