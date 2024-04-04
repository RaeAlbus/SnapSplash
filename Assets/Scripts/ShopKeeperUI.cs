using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopKeeperUI : MonoBehaviour
{
    // Singleton instance of the ShopKeeperUI
    private static ShopKeeperUI _instance;

    // Read-only instant of ShopKeeperUI to access from other scripts
    public static ShopKeeperUI Instance => _instance;

    // Text Scenes
    public string[] initTexts;

    public string[] exitTexts;

    // Accessibles for the UI
    public TextMeshProUGUI dialougeText;
    public Canvas dialougeCanvas;
    public TextMeshProUGUI clickToContinueText;
    public TextMeshProUGUI clickToContinueText2;
    public Button sellBtn;
    public Button buyBtn;
    public Image dialougePanel;
    public Image shopPanel;
    
    private bool currTalking;

    private void Start()
    {

        if(_instance == null)
        {
            _instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }

        currTalking = false;
        dialougeCanvas.enabled = false;

    }

    public void InitDialouge()
    {
        // Go into dialouge mode and show dialouge box
        currTalking = true;
        dialougeCanvas.enabled = true;

        // Hidden until triggered to appear
        clickToContinueText.enabled = false;
        shopPanel.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        buyBtn.gameObject.SetActive(false);

        StartCoroutine(DisplayTextsSequentially(initTexts, 0, () =>
        {
            clickToContinueText.enabled = false;

            sellBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }));
    }

    public void ExitDialouge()
    {
        currTalking = true;
        dialougeCanvas.enabled = true;
        clickToContinueText.enabled = false;

        StartCoroutine(DisplayTextsSequentially(exitTexts, 0, () =>
        {
            dialougeCanvas.enabled = false;
        }));
    }

    public void SoldFishDialouge(float money)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        string moneyString = money.ToString("F2");
        string howMuchSold = "Wow! These are some nice photos! Here's $" + moneyString + " for them";
        string end = "Pleasure doing business with ya!";
        string[] soldDialouge = {howMuchSold, end};

        StartCoroutine(DisplayTextsSequentially(soldDialouge, 0, () =>
        {
            WhatsNextDialouge();
        }));
    }

    public void NoFishDialouge()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        string error = "Oh looks like you don't have any photos to sell me";
        string instructions = "Why don't you go down under again? I'll wait here!";
        string[] notSoldDialouge = {error, instructions};

        StartCoroutine(DisplayTextsSequentially(notSoldDialouge, 0, () =>
        {
            WhatsNextDialouge();
        }));
    }

    public void BuyItemDialouge()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        string error = "Great! You came to the right place";
        string instructions = "I'll open up my shop, just click on what catches your eye";
        string[] notSoldDialouge = {error, instructions};

        StartCoroutine(DisplayTextsSequentially(notSoldDialouge, 0, () =>
        {
            dialougePanel.gameObject.SetActive(false);
            shopPanel.gameObject.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }));
    }

    public void ExitShopDialouge()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        string justwordsatthispointguysidk = "Hope you got what you need";
        string[] exitShop = {justwordsatthispointguysidk};

        StartCoroutine(DisplayTextsSequentially(exitShop, 0, () =>
        {
            WhatsNextDialouge();
        }));
    }

    public void WhatsNextDialouge()
    {
        dialougePanel.enabled = true;
        string prompt = "So, what are you interested in doing today?";
        string[] whatsNext = {prompt};

        StartCoroutine(DisplayTextsSequentially(whatsNext, 0, () =>
        {
            clickToContinueText.enabled = false;

            sellBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }));
    }

    private IEnumerator DisplayTextsSequentially(string[] texts, int index, Action onComplete)
    {
        if (index >= texts.Length)
        {
            // All texts displayed, stop dialogue
            currTalking = false;
            onComplete?.Invoke();
            yield break;
        }

        dialougeText.text = texts[index];
        dialougeText.maxVisibleCharacters = 0;

        bool displayAllText = false;
        while (!displayAllText && dialougeText.maxVisibleCharacters < texts[index].Length)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Display the entire text if the player left-clicks
                dialougeText.maxVisibleCharacters = texts[index].Length;
                displayAllText = true;
            }
            else
            {
                dialougeText.maxVisibleCharacters++;
            }

            yield return new WaitForSeconds(0.05f);
        }

        clickToContinueText.enabled = true;

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        clickToContinueText.enabled = false;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        StartCoroutine(DisplayTextsSequentially(texts, index + 1, onComplete));
    }

}
