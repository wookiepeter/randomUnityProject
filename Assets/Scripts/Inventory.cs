using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDataBase database;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    int slotAmount;
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        database = GetComponent<ItemDataBase>(); //Referenz auf das Skript
        slotAmount = 6; //SlotGröße
        inventoryPanel = GameObject.Find("InventoryPanel"); //Referenz auf das InventoryPanel
        slotPanel = inventoryPanel.transform.FindChild("SlotPanel").gameObject; //Referenz über das InventoryPanel auf das SlotPanel
        for(int i = 0; i < slotAmount; i++) 
        {//--Erstellt die Slots!
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));//BlankSlotCreate
            slots[i].transform.SetParent(slotPanel.transform); //Position durch VererbungGesetzt
        }

        //TestReihe:
        AddItem(0);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(1);
        AddItem(0);
        AddItem(1);//1-x ID
    }

    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id); //Sucht das Item per ID raus
        for (int i = 0; i < items.Count; i++)
        {
            //EmptyItem
            if (items[i].ID == -1) //--Wenn die ID gefunden wurde ->
            {
                items[i] = itemToAdd;
                GameObject itemObj = Instantiate(inventoryItem); //item korrekter Slot
                itemObj.transform.SetParent(slots[i].transform);

                itemObj.transform.position = new Vector2(5,-5); //###############Position des Sprites
                itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                itemObj.name = itemToAdd.Title;

                break; //ABbrechen, nachdems gefunden wurde
            }
        }
    }
}
