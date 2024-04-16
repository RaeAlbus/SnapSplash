using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseSensitivity : MonoBehaviour
{
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValue;

    private void Start()
    {
        sensitivitySlider.value = PlayerPrefs.GetInt("mouseSensitivity", 175);
        sensitivitySlider.onValueChanged.AddListener(OnSliderValueChanged);
        sensitivityValue.text = PlayerPrefs.GetInt("mouseSensitivity", 175).ToString();
    }

    private void OnSliderValueChanged(float value)
    {
        int wholeNumVal = (int) value;
        sensitivityValue.text = wholeNumVal.ToString();
        CameraController.UpdateMouseSensitivity(wholeNumVal);
    }

}
