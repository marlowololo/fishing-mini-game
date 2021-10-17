using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {
    [SerializeField] private Slider _fishingCastMeter;

    public void ShowFishingCastingMeter() {
        _fishingCastMeter.gameObject.SetActive(true);
    }

    public void SetFishingCastingMeterValue(float value) {
        _fishingCastMeter.value = value;
    }

    public void HideFishingCastingMeter() {
        _fishingCastMeter.gameObject.SetActive(false);
    }
}
