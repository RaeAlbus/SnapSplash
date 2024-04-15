using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffect : MonoBehaviour
{
    public float flashDuration = 0.1f;
    public float flashInterval = 0.05f;

    private Image flashImage;

    // Start is called before the first frame update
    void Start()
    {
        flashImage = GetComponent<Image>();
        flashImage.enabled = false;
    }

    public void StartFlash()
    {
        StartCoroutine(Flash());
    }

    public void StopFlash()
    {
        StopCoroutine(Flash());
        flashImage.enabled = false;
    }

    private IEnumerator Flash()
    {
        flashImage.enabled = true;
        Color originalColor = flashImage.color;
        float startTime = Time.time;

        while (Time.time - startTime < flashDuration)
        {
            float alpha = Mathf.PingPong((Time.time - startTime) / flashDuration, 1f);
            flashImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        flashImage.color = originalColor;
        flashImage.enabled = false;
    }


}
