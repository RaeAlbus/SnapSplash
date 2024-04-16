using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetStats : MonoBehaviour
{
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        text.SetText("Total Fish Photographed: " + 
            PlayerPrefs.GetInt("FishPhotographed", 0) +
            "\n\nTotal Coins Collected: " +
            PlayerPrefs.GetFloat("CoinsCollected", 0).ToString("F2"));
    }
}
