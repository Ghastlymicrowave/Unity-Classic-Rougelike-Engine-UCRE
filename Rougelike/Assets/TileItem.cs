using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileItem : MonoBehaviour
{
    public List<InventoryItem> items;
    Tileboard tileboardReference;
    int xCord;
    int yCord;

    public void set(InventoryItem newItem, int x, int y, Tileboard tileboard)
    {
        items = new List<InventoryItem>();
        items.Add(newItem.ItemClone());
        xCord = x;
        yCord = y;
        tileboardReference = tileboard;
        
        //set image
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(tileboardReference.currentXrot, 0f, 0f);
    }

}
