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

    // Accessibles for the UI
    public TextMeshProUGUI dialougeText;
    public Canvas dialougeCanvas;
    public TextMeshProUGUI clickToContinueText;
    public Button sellBtn;
    public Button buyBtn;
    public Image dialougePanel;
    public Image shopPanel;

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

        dialougeCanvas.gameObject.SetActive(false);

    }

    public void InitDialouge()
    {
        // Show cursor and show dialouge box
        SetMouseFree(true);
        dialougeCanvas.gameObject.SetActive(true);

        // Hide these btns until the right dialouge prompts them on
        clickToContinueText.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        buyBtn.gameObject.SetActive(false);

        string[] initTexts = {"Hello! Welcome to my shop!", "What can I do ya for today?"};

        // Start dialouge coroutine
        StartCoroutine(DisplayTextsSequentially(initTexts, 0, () =>
        {
            // Show sell/buy btns to prompt user once coroutine ends
            sellBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(true);
        }));
    }

    public void ExitDialouge()
    {
        // Hide cursor
        SetMouseFree(false);

        // Turn off all UI elements to avoid it showing up when rentering dialouge
        ShopKeeperBehavior.inShop = false;
        dialougeCanvas.gameObject.SetActive(false);
    }

    public void SoldFishDialouge(float money)
    {
        string moneyString = money.ToString("F2");
        string[] soldDialouge = {"Wow! These are some nice photos! Here's $" + moneyString + " for them", "Pleasure doing business with ya!"};

        StartCoroutine(DisplayTextsSequentially(soldDialouge, 0, () =>
        {
            WhatsNextDialouge();
        }));
    }

    public void NoFishDialouge()
    {
        string[] notSoldDialouge = {"Oh looks like you don't have any photos to sell me", "Why don't you go down under again? I'll wait here!"};

        StartCoroutine(DisplayTextsSequentially(notSoldDialouge, 0, () =>
        {
            WhatsNextDialouge();
        }));
    }

    public void BuyItemDialouge()
    {
        string[] notSoldDialouge = {"Great! You came to the right place"}; //, "I'll open up my shop, just click on what catches your eye"};

        StartCoroutine(DisplayTextsSequentially(notSoldDialouge, 0, () =>
        {
            dialougePanel.gameObject.SetActive(false);
            shopPanel.gameObject.SetActive(true);
        }));
    }

    public void ExitShopDialouge()
    {
        dialougePanel.gameObject.SetActive(true);
        shopPanel.gameObject.SetActive(false);

        string leftShopDialouge = "Hope you got what you need";
        string[] exitShop = {leftShopDialouge};

        StartCoroutine(DisplayTextsSequentially(exitShop, 0, () =>
        {
            WhatsNextDialouge();
        }));
    }

    public void WhatsNextDialouge()
    {
        string prompt = "So, what are you interested in doing today?";
        string[] whatsNext = {prompt};

        StartCoroutine(DisplayTextsSequentially(whatsNext, 0, () =>
        {
            sellBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(true);
        }));
    }

   private IEnumerator DisplayTextsSequentially(string[] texts, int index, Action onComplete)
   {
        // Hide all non dialouge UI objects when speaking
        clickToContinueText.gameObject.SetActive(false);
        shopPanel.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        buyBtn.gameObject.SetActive(false);

        if(index >= texts.Length)
        {
            onComplete?.Invoke();
            yield break;
        }

        dialougeText.text = texts[index];
        dialougeText.maxVisibleCharacters = 0;

        foreach(char letter in texts[index].ToCharArray())
        {
            //PlayDialougeSound(dialougeText.maxVisibleCharacters);
            dialougeText.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.05f);
        }

        // Prompts user to left-click to go to next line
        clickToContinueText.gameObject.SetActive(true);
        // Waits for left-click
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        // Goes to next line
        StartCoroutine(DisplayTextsSequentially(texts, index + 1, onComplete));
   }

    public void SetMouseFree(bool mouseMode)
    {
        // if true --> set mouse free
        if(mouseMode)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
