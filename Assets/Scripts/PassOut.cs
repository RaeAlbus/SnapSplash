using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassOut : MonoBehaviour
{

    public float fadeSpeed = 1f;
    private Image fadeImage;

    // Start is called before the first frame update
    void Start()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.isLevelLost)
        {
            // increase the alpha of the image
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(fadeImage.color.a, 1, fadeSpeed * Time.deltaTime));
        }
    }
}
