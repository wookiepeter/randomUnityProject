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
            slots[i].GetComponent<Slot>().id = i;
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
        if (itemToAdd.Stackable && CheckIfItemIsInInventory(itemToAdd)) //Schaut nach ob ein Item Stackbar ist
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    data.amount++;
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString(); //Gibt die Anzahl als Text für das Child             }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                //EmptyItem
                if (items[i].ID == -1) //--Wenn die ID gefunden wurde ->
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem); //item korrekter Slot
                    itemObj.GetComponent<ItemData>().item = itemToAdd;
                    itemObj.GetComponent<ItemData>().slot = i;
                    itemObj.transform.SetParent(slots[i].transform);

                    itemObj.transform.position = new Vector2(5, -5); //###############Position des Sprites
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
                    itemObj.name = itemToAdd.Title;

                    break; //ABbrechen, nachdems gefunden wurde
                }
            }
        }
       
    }//endeAddItem

    bool CheckIfItemIsInInventory(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
                 if (items[i].ID == item.ID)
                {
                    return true;
                }
         }
           

        
        return false;
    }
}
