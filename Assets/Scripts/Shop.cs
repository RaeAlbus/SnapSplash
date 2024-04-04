using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject itemPrefab;
    public List<Item> stock = new List<Item>();

    void Start()
    {
        stock.Add(new Item("Test", 5.0f));
    }

    void DisplayItems()
    {
        foreach(Item item in stock)
        {
            GameObject newButton = Instantiate(itemPrefab, this.gameObject.transform);
            newButton.GetComponentInChildren<Text>().text = "Click me!";
        }
    }

    void BuyItem(string name)
    {

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