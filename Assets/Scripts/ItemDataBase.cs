using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDataBase : MonoBehaviour {
    private List<Item> database = new List<Item>(); //Liste mit Items
	private JsonData itemData;
	
    void Start()
    {
        //zb Element auf 0 setzen
        //Item item = new Item(0, "Sword", 5, 3);
        //database.Add(item);
        //debug.log(database[0].Title);

        //--Json ist quasi ein textdokument wo alle Gegenstände aufgelistet werden - Referenz wird geholt
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Item.json")); 
        //--erstellt die gegenstände
        ConstrucItemDatabase();
            
    }
    public Item FetchItemByID(int id)
    {
        for(int i = 0; i < database.Count; i++)
        {
            if(database[i].ID == id)
            {
                return database[i];
            }
            
        }
        return null;
    }


    void ConstrucItemDatabase()
    {
        for(int i = 0; i< itemData.Count; i++)
        {
            //--Konstruktor zum erstellen der Items
            database.Add(new Item((int)itemData[i]["id"], itemData[i]["title"].ToString(), (int)itemData[i]["value"],
                                    (int)itemData[i]["stats"]["power"], (int)itemData[i]["stats"]["defence"], (int)itemData[i]["stats"]["vitality"],
                                    itemData[i]["description"].ToString(), (bool)itemData[i]["stackable"],
                                    (int)itemData[i]["rarity"], itemData[i]["slug"].ToString()));
        }
    }
}

public class Item
{
    
    public int ID { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
    public int Power { get; set; }
    public int Defence { get; set; }
    public int Vitality { get; set; }
    public string Description { get; set; }
    public bool Stackable { get; set; }
    public int Rarity { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }

    public Item(int id, string title, int value, int power, int defence, int viality, string description, bool stackable, int rarity, string slug)
    {
        this.ID = id;
        this.Title = title;
        this.Value = value;
        this.Power = power;
        this.Vitality = viality;
        this.Description = description;
        this.Stackable = stackable;
        this.Rarity = rarity;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Items/" +slug);

    }

   public Item()
   {
       this.ID = -1;
   }
}