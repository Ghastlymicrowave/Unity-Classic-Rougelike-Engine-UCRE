using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRender : MonoBehaviour
{
    private Tileboard tileboard;

    private List<InventoryItem> inventory;

    //panels
    public GameObject HUDInventory;
    Text HUDInventoryText;
    public GameObject MENUInventory;
    Text MENUInventoryText;

    int? cursorPosition = null;
    public enum UIPanels
    {
        HUDInventory,
        InventoryMenu
    }

    // Start is called before the first frame update
    void Start()
    {
        HUDInventoryText = HUDInventory?.transform.GetChild(0).GetComponent<Text>();
        MENUInventoryText = MENUInventory?.transform.GetChild(0).GetComponent<Text>();

        tileboard = GetComponent<Tileboard>();
        tileboard.inventoryUpdated = (List<InventoryItem> iv) => { this.PlayerInventoryUpdated(iv); };
        tileboard.acceptingInput = true;
        ActivateOnlyThisPanel(UIPanels.HUDInventory);
    }

    public GameObject GetPanelOrNull(UIPanels panel)
    {
        switch (panel)
        {
            case UIPanels.HUDInventory: 
                if (HUDInventory != null) { return HUDInventory.gameObject; } else goto default;
            case UIPanels.InventoryMenu: 
                if (MENUInventory != null) { return MENUInventory.gameObject; } else goto default;
            default:
                Debug.LogWarning("no panel for " + panel.ToString() + " found, returned null"); 
                return null;
        }
    }

    public void DisableAllPanels()
    {
        for (int i = 0; i < typeof(UIPanels).GetEnumNames().Length; i++)
        {
            GetPanelOrNull((UIPanels)typeof(UIPanels).GetEnumValues().GetValue(i))?.SetActive(false);
        }
    }

    public void SetPanelActive(UIPanels panel,bool active)
    {
        GetPanelOrNull(panel)?.SetActive(active);
    }

    public void ActivateOnlyThisPanel(UIPanels panel)
    {
        DisableAllPanels();
        SetPanelActive(panel, true);
    }

    public void PlayerInventoryUpdated(List<InventoryItem> iv)
    {
        string outstring ="";
        foreach (InventoryItem item in iv) {
            if (item.equipable)
            {
                switch (item.GetType().Name)
                {
                    case nameof(WeaponItem):
                        WeaponItem newItem = (WeaponItem)item;
                        if (newItem.equipped)
                        {
                            outstring += "<equipped-"+ newItem.equippedPosition.ToString();
                        }
                        break;
                    default:
                        break;
                }
            }
            outstring += item.name + " - " + item.description + "\n";
        }
        HUDInventoryText.text= outstring;
        inventory = iv;
    }

    public void UpdateInventoryUI()
    {
        if (cursorPosition == null || cursorPosition < 0) { cursorPosition = 0; }
        if (cursorPosition > inventory.Count - 1) { cursorPosition = inventory.Count - 1; }
        string outstring = "";
        for (int i = 0; i < inventory.Count; i++)
        {
            if (cursorPosition == i) { outstring += ">>> "; } 
            else
            { outstring += "       "; }
            InventoryItem item = inventory[i];
            switch (item.GetType().Name)
            {
                case nameof(WeaponItem):
                    WeaponItem newItem = (WeaponItem)item;
                    if (newItem.equipped)
                    {
                        outstring += "<equipped to " + newItem.equippedPosition.ToString() +"> ";
                    }
                    break;
                default:
                    break;
            }
            outstring += " " + item.name + "\n";
            
        }
        MENUInventoryText.text = outstring;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (GetPanelOrNull(UIPanels.InventoryMenu)?.activeSelf ?? false)
            {
                ActivateOnlyThisPanel(UIPanels.HUDInventory);
                tileboard.acceptingInput = true;
            }
            else if (GetPanelOrNull(UIPanels.InventoryMenu)!=null)
            {
                ActivateOnlyThisPanel(UIPanels.InventoryMenu);
                tileboard.acceptingInput = false;
                UpdateInventoryUI();
            }
        }

        if (GetPanelOrNull(UIPanels.InventoryMenu)?.activeSelf ?? false)
        {
            if (Input.GetButtonDown("north"))
            {
                cursorPosition -= 1;
                UpdateInventoryUI();
            }else if (Input.GetButtonDown("south"))
            {
                cursorPosition += 1;
                UpdateInventoryUI();
            }
        }
    }
}
