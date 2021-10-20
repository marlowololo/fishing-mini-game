using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PondController : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] float _fishSpawnWaitTime = 3f; //specified in test

    [Header("Prefabs")]
    [SerializeField] Fish _fishPrefabs;

    private Bait _bait;
    private Coroutine _startFishingCoroutine;
    private bool _fishingStarted = false;

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("Bait") && !_fishingStarted) {
            _bait = other.gameObject.GetComponent<Bait>();
            _bait.AddOnBaitPulledCallback(OnBaitPulled);

            _fishingStarted = true;
            _startFishingCoroutine = StartCoroutine(StartFishing());
        }
    }

    private IEnumerator StartFishing() {
        yield return new WaitForSeconds(_fishSpawnWaitTime);
        Fish fish = Instantiate<Fish>(_fishPrefabs);
        Vector3 fishSpawnPosOffset = Random.onUnitSphere;
        fishSpawnPosOffset.y = 0;
        fish.transform.position = _bait.transform.position + fishSpawnPosOffset;
        fish.ChaseBait(_bait);
        _bait.AddOnBaitPulledCallback(fish.OnBaitPulled);

        _fishingStarted = false;
    }

    private void OnBaitPulled() {
        _fishingStarted = false;
        StopCoroutine(_startFishingCoroutine);
    }
}
