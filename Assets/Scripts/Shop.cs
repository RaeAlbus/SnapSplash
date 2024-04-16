using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Shop : MonoBehaviour
{
    // Button prefabs of all items to display in shop
    public GameObject itemPrefab;
    public GameObject soldOutItemPrefab;

    // Everything this shop has ever sold/is selling
    public List<Item> stock = new List<Item>();
    public List<Sprite> icons = new List<Sprite>();

    public float spacing = 1f;

    // Current button that has been pressed in shop
    private GameObject buttonObject;

    void Start()
    {
        stock.Add(new Item("SD Card\n(+Storage)", 450.0f, icons.FirstOrDefault(sprite => sprite.name == "StorageIcon")));
        stock.Add(new Item("Air Tank\n(+Time)", 600.0f, icons.FirstOrDefault(sprite => sprite.name == "AirIcon")));
        stock.Add(new Item("Depth Gauge\n(+Unlock Level)", 300.0f, icons.FirstOrDefault(sprite => sprite.name == "DepthIcon")));
        stock.Add(new Item("Fins\n(+Speed)", 500.0f, icons.FirstOrDefault(sprite => sprite.name == "FinsIcon")));
        stock.Add(new Item("Flashlight\n(Item)", 500.0f, icons.FirstOrDefault(sprite => sprite.name == "FlashlightIcon")));
        GenerateShop();
    }

    public void GenerateShop()
    {
        Transform panelTransform = transform.Find("ShopBG");
        Vector2 itemSize = itemPrefab.GetComponent<RectTransform>().sizeDelta;

        float totalWidth =  stock.Count * itemSize.x + (stock.Count - 1) * spacing;
        float startX = -totalWidth / 2f + itemSize.x / 2f;

        for (int i = 0; i <  stock.Count; i++)
        {
            if(!stock[i].hasBought)
            {
                // Calculate the position for the current button
                Vector3 position = new Vector3(startX + i * (itemSize.x + spacing), -50f, 0f);

                // Instantiate the button at the calculated position
                GameObject buttonInstance = Instantiate(itemPrefab, panelTransform.position + position, Quaternion.identity);
                buttonInstance.name = stock[i].name;
                buttonInstance.transform.SetParent(panelTransform);

                TextMeshProUGUI nameText = buttonInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                nameText.text = stock[i].name;

                TextMeshProUGUI priceText = buttonInstance.transform.Find("Price").GetComponent<TextMeshProUGUI>();
                priceText.text = "$" + stock[i].price.ToString();

                Image icon = buttonInstance.transform.Find("Image").GetComponent<Image>();
                icon.sprite = stock[i].icon;

                Button button = buttonInstance.GetComponent<Button>();
                button.onClick.AddListener(() => OnButtonClick(button));
            }
        }

    }

    void OnButtonClick(Button clickedButton)
    {
        BuyItem(clickedButton.gameObject.name);
    }

    public void BuyItem(string name)
    {
        foreach (Item item in stock)
        {
            if (item.name == name)
            {
                // Gets reference to UI button for the item
                buttonObject = GameObject.Find(item.name);
                Button button = buttonObject.GetComponent<Button>();

                if(item.price <= LevelManager.money && !item.hasBought)
                {
                    // Buys the item
                    SoundManager.Instance.PlayCashSFX();
                    LevelManager.money -= item.price;
                    UpdateValues(item);

                    // Makes UI button of item green to display it has been bought
                    ColorBlock colors = button.colors;
                    colors.pressedColor = Color.green;
                    colors.selectedColor = Color.green;
                    button.colors = colors;

                    Invoke("ResetButton", 1.0f);
                    return;
                }
                else if(item.hasBought)
                {
                    SoldOut(item);
                } 
                else
                {
                    // Can not buy the item due to insufficent cash or has bought already
                    SoundManager.Instance.PlayNoStorageSFX();

                    // Makes UI button of item red to display it has not been bought
                    ColorBlock colors = button.colors;
                    colors.pressedColor = Color.red;
                    colors.selectedColor = Color.red;
                    button.colors = colors;

                    Invoke("ResetButton", 1.0f);
                    return;
                }

                break;
            } 
        }

    }

    void ResetButton()
    {
        Button button = buttonObject.GetComponent<Button>();
        
        ColorBlock colors = button.colors;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        button.colors = colors;
    }


    // This method is meant to take the upgrade earned by buying this item and updating
    // the appropriate methods
    public void UpdateValues(Item item)
    {
        string[] itemVals = item.name.Split('\n');
        string name = itemVals[0];

        switch (name)
        {
            case "SD Card":
                LevelManager.storageLeft += 4;
                LevelManager.totalStorage += 4;
                break;

            case "Air Tank":
                LevelManager.totalAir += 20;
                break;

            case "Depth Gauge":
                if(LevelManager.maxDepth <= -100)
                {
                    item.hasBought = true;
                }
                LevelManager.maxDepth -= 40;
                break;

            case "Fins":
                PlayerController.movementSpeedUnderwater += 2f;
                break;

            case "Flashlight":
                LevelManager.hasFlashlight = true;
                item.hasBought = true;

                break;

            default:
                break;
        }
    }

    public void SoldOut(Item item)
    {
        Transform panelTransform = transform.Find("ShopBG");

        // Gets reference to UI button for the item
        buttonObject = GameObject.Find(item.name);
        Button button = buttonObject.GetComponent<Button>();
        Vector3 btnPos = button.transform.position;

        GameObject soldOutButtonInstance = Instantiate(soldOutItemPrefab, btnPos, Quaternion.identity);
        soldOutButtonInstance.name = item.name;
        soldOutButtonInstance.transform.SetParent(panelTransform);

        TextMeshProUGUI nameText = soldOutButtonInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        nameText.text = item.name;

        TextMeshProUGUI priceText = soldOutButtonInstance.transform.Find("Price").GetComponent<TextMeshProUGUI>();
        priceText.text = "$---";
    }

}

public class Item
{
    public string name;
    public float price;
    public Sprite icon;
    public bool hasBought;

    public Item(string name, float price, Sprite icon)
    {
        this.name = name;
        this.price = price;
        this.icon = icon;
        this.hasBought = false;
    }
}