using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TurnSpeedManager : MonoBehaviour
{
    Slider slider;
    private void Start()
    {
        slider = GetComponent<Slider>();

        if (!PlayerPrefs.HasKey("turnSpeed"))
        {
            UpdateTurnSpeed(1);
            slider.value = 1;
        }
    }

    void UpdateTurnSpeed(float value)
    {
        PlayerPrefs.SetFloat("turnSpeed", value);
        Board.instance.ActionTimeMultiplier = 1f/value;
    }

    public void OnValueChange() {
        UpdateTurnSpeed(slider.value);
    }
}
