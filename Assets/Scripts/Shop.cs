using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    // Button prefabs of all items to display in shop
    public GameObject itemPrefab;

    // Everything this shop has ever sold/is selling
    public List<Item> stock = new List<Item>();

    // Current button that has been pressed in shop
    private GameObject buttonObject;

    void Start()
    {
        stock.Add(new Item("40 Air", 450.0f));
        stock.Add(new Item("60 Air", 600.0f));
        stock.Add(new Item("20 Storage", 300.0f));
        stock.Add(new Item("24 Storage", 500.0f));
    }

    public void BuyItem(string name)
    {
        foreach (Item item in stock)
        {
            if (item.name == name && !item.hasBought)
            {
                // Gets reference to UI button for the item
                buttonObject = GameObject.Find(item.name);
                Button button = buttonObject.GetComponent<Button>();

                if(item.price <= LevelManager.money)
                {
                    // Buys the item
                    SoundManager.Instance.PlayCashSFX();
                    LevelManager.money -= item.price;
                    item.hasBought = true;
                    UpdateValues(item);

                    // Makes UI button of item green to display it has been bought
                    ColorBlock colors = button.colors;
                    colors.pressedColor = Color.green;
                    colors.selectedColor = Color.green;
                    button.colors = colors;

                    Invoke("ResetButton", 1.0f);
                    return;
                }
                else
                {
                    // Can not buy the item due to insufficent cash
                    SoundManager.Instance.PlayNoStorageSFX();

                    // Makes UI button of item red to display it has not been bought
                    ColorBlock colors = button.colors;
                    colors.pressedColor = Color.red;
                    colors.selectedColor = Color.red;
                    button.colors = colors;

                    Invoke("ResetButton", 1.0f);
                    return;
                }
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
        string[] itemVals = item.name.Split(' ');
        
        if(itemVals[1] == "Air")
        {
            LevelManager.totalAir = int.Parse(itemVals[0]);
        }
        else if(itemVals[1] == "Storage")
        {
            LevelManager.storageLeft += (int.Parse(itemVals[0]) - LevelManager.totalStorage);
            LevelManager.totalStorage = int.Parse(itemVals[0]);
        }
    }

}

public class Item
{
    public string name;
    public float price;
    public bool hasBought;

    public Item(string name, float price)
    {
        this.name = name;
        this.price = price;
        this.hasBought = false;
    }
}