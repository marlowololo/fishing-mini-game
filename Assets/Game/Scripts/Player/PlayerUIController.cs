using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {
    [SerializeField] private Slider _fishingCastMeter;
    [SerializeField] private Image _fishingCastMeterFill;
    [SerializeField] private GameObject _baitTakenIndicator;

    private Animation _baitTakenIndicatorAnimation;

    private void Start() {
        _baitTakenIndicatorAnimation = _baitTakenIndicator.GetComponent<Animation>();
    }

    public void ShowFishingCastingMeter(Vector3 uiRefPosition) {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(uiRefPosition);
        Vector3 targetPosition = _fishingCastMeter.transform.position;
        targetPosition.y = screenPosition.y;
        _fishingCastMeter.gameObject.SetActive(true);
        _fishingCastMeter.transform.position = targetPosition;
    }

    public void SetFishingCastingMeterValue(float value) {
        _fishingCastMeter.value = value;
        _fishingCastMeterFill.color = Color.Lerp(Color.yellow, Color.green, value);
    }

    public void HideFishingCastingMeter() {
        _fishingCastMeter.gameObject.SetActive(false);
    }

    public void OnBaitTaken(Vector3 uiRefPosition) {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(uiRefPosition);
        _baitTakenIndicator.SetActive(true);
        _baitTakenIndicator.transform.position = screenPosition;
        _baitTakenIndicatorAnimation.Play();
    }

    public void HideBaitIndicator() {
        _baitTakenIndicator.SetActive(false);
    }
}
