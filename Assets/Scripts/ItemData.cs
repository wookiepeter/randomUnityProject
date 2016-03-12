using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //Speichern die Events, zb ich klicke auf ein Objekt und kann es bewegen - 
    //wichtig, wir schieben das Item vor das parent objekt und EndDrag wieder zurück

    public Item item;
    public int amount;
    public int slot;

    private Inventory inv;
    private ToolTip toolTip;
    //private Transform originalParent;
    private Vector2 offset;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        toolTip = inv.GetComponent<ToolTip>();
    }
     public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {//--Wir haben ein item:
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            //originalParent = this.transform.parent; //Speicher die OriginalGröße des ParentPanelab
            this.transform.SetParent(this.transform.parent.parent);
            this.transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {//--Wir haben ein item:
            this.transform.position = eventData.position -offset;
        }
    }

   

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inv.slots[slot].transform);
        this.transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.Activate(item); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.Deactivate();
    }
}
