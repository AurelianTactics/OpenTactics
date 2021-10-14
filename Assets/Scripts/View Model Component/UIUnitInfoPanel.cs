using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Ugly placeholder that holds information and stats about units
/// </summary>
/// <remarks>
/// At some point, need to make allow players to navigate and request help on various terms and stats
/// </remarks>
public class UIUnitInfoPanel : MonoBehaviour {

    public Text nameText;
    public Text lifeText;
    public Text speedText;
    public Text strengthText;
    public Text courageText;
    public Text wpText;
    public Text evadeText;
    public Text statusText;
    public Text innateText;

    public Text primaryText;
    public Text secondaryText;
    public Text reactionText;
    public Text supportText;
    public Text movementText;

    public Text weaponText;
    public Text offhandText;
    public Text headText;
    public Text bodyText;
    public Text accessoryText;

    public Image genderImage;
    public Button backButton;
    //UIBackButton backButtonUI;
    private bool disableClose = false;

    public GameObject detailsPanel;
    public Text detailsTitle;
    public Text detailsDetails;

    PlayerUnit puFile = null;

    // Use this for initialization
    void Start () {
        //backButtonUI = backButton.GetComponent<UIBackButton>();
    }

    public void Open(bool zDisableClose = false)
    {
        //Debug.Log(" asdf " + zDisableClose);
        gameObject.SetActive(true);
        //disableClose = zDisableClose; Debug.Log(" asdf " + disableClose);
    }

    public void Close()
    {
        if(!disableClose)
        {
            //SceneCreate.menuMenu.ShowStatsMenu();
            gameObject.SetActive(false);
        } 
    }

    public void ActBack()
    {
        //Debug.Log("Asdf");
        //closes this menu and reopens the other menu
        Close();
        //MapTileManager.Instance.UnhighlightAllTiles();
    }

    public void PopulatePlayerInfo(PlayerUnit pu, bool isCombat = true )
    {
        Open(disableClose);
        CloseDetailsPanel();
        puFile = pu;
        nameText.text = pu.UnitName; //+ " Team: " + pu.TeamId + " Level: " + pu.Level + "Sex/Color: " + pu.Sex + "/" + pu.ZodiacInt;
        lifeText.text = "HP: " + pu.StatTotalLife + "/" + pu.StatTotalMaxLife + " MP: " + pu.StatTotalMP + "/" + pu.StatTotalMaxMP;
        speedText.text = "Speed: " + pu.StatTotalSpeed + " Move: " + pu.StatTotalMove + " Jump: " + pu.StatTotalJump;
        strengthText.text = "STR: " + pu.StatTotalPA + " INT: " + pu.StatTotalMA + " AGI: " + pu.StatTotalAgi;

        courageText.text = "CRG: " + pu.StatTotalBrave + " WIS: " + pu.StatTotalFaith + " SKL: " + pu.StatTotalCunning;
        wpText.text = "LVL: " + pu.Level + " WP: " + pu.GetWeaponPower(false, true) + " TP: " + pu.CT + " TO: " + pu.TurnOrder; // + " Evade: " + pu.StatTotalCEvade;
        evadeText.text = "Evade (C/OP/OM/AP/AM/W): " + pu.StatTotalCEvade + "/" + pu.StatItemOffhandPEvade + "/" + pu.StatItemOffhandMEvade
            + "/" + pu.StatItemAccessoryPEvade + "/" + pu.StatItemAccessoryMEvade + "/" + pu.StatItemWEvade;

        primaryText.text = "" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId); //Debug.Log("" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId));
        secondaryText.text = "" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SECONDARY, pu.AbilitySecondaryCode);
        reactionText.text = "" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_REACTION, pu.AbilityReactionCode);
        supportText.text = "" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_SUPPORT, pu.AbilitySupportCode);
        movementText.text = "" + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_MOVEMENT, pu.AbilityMovementCode);

        weaponText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotWeapon, NameAll.ITEM_SLOT_WEAPON); //Debug.Log("getting item number " + pu.ItemSlotWeapon);
        offhandText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotOffhand, NameAll.ITEM_SLOT_OFFHAND);
        headText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotHead, NameAll.ITEM_SLOT_HEAD);
        bodyText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotBody, NameAll.ITEM_SLOT_BODY);
        accessoryText.text = "" + ItemManager.Instance.GetItemName(pu.ItemSlotAccessory, NameAll.ITEM_SLOT_ACCESSORY);

        string zString = "Statuses: ";
        if (isCombat) //builds statuses from lasting statuses
        {
            List<string> statusList = new List<string>();
            StatusManager.Instance.GetUnitStatusList(pu.TurnOrder, statusList);
            //Debug.Log("asdf " +statusList.Count);
            foreach (string s in statusList)
            {
                zString += s + ", ";
            }
            //Debug.Log("asdf");
            //change background for teamId
            if (pu.TeamId == NameAll.TEAM_ID_GREEN)
            {
                //var rend = this.gameObject.GetComponent<Renderer>();
                //rend.material.mainTexture = Resources.Load("menu_team_1") as Texture;
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
            }
            else if (pu.TeamId == NameAll.TEAM_ID_RED)
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_2");
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
            }
        }
        else //builds statuses from items
        {
            zString += ItemManager.Instance.GetItemStatusNames(pu);
            string zString2 = AbilityManager.Instance.GetAbilityStatusNames(pu);
            if (zString2.Length > 0)
            {
                if (zString.Length > 10)
                {
                    zString += "," + zString2;
                }
                else
                {
                    zString += zString2;
                }
            }
            
            
            //Debug.Log("asdf2");
        }
        statusText.text = zString;

        innateText.text = "Innate: " + AbilityManager.Instance.GetInnateString(pu.ClassId);

        zString = ""; //Debug.Log("pu sex is " + pu.Sex + ",");
        if (pu.Sex == "Male")
        {
            zString = "male_" + (pu.ZodiacInt + 1);
        }
        else
        {
            zString = "female_" + (pu.ZodiacInt + 1);
        }
        genderImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Zodiac/" + zString); //Debug.Log("Sprites/Zodiac/" + zString);
    }

    public void OnItemWeaponClick()
    {
        PopulateDetailsImage("item", NameAll.ITEM_SLOT_WEAPON);
    }

    public void OnItemOffhandClick()
    {
        PopulateDetailsImage("item", NameAll.ITEM_SLOT_OFFHAND);
    }

    public void OnItemHeadClick()
    {
        PopulateDetailsImage("item", NameAll.ITEM_SLOT_HEAD);
    }

    public void OnItemBodyClick()
    {
        PopulateDetailsImage("item", NameAll.ITEM_SLOT_BODY);
    }

    public void OnItemAccessoryClick()
    {
        PopulateDetailsImage("item", NameAll.ITEM_SLOT_ACCESSORY);
    }

    public void OnAbilityPrimary()
    {
        PopulateDetailsImage("ability", NameAll.ABILITY_SLOT_PRIMARY);
    }

    public void OnAbilitySecondary()
    {
        PopulateDetailsImage("ability", NameAll.ABILITY_SLOT_SECONDARY);
    }

    public void OnAbilityReaction()
    {
        PopulateDetailsImage("ability", NameAll.ABILITY_SLOT_REACTION);
    }

    public void OnAbilitySupport()
    {
        PopulateDetailsImage("ability", NameAll.ABILITY_SLOT_SUPPORT);
    }

    public void OnAbilityMovement()
    {
        PopulateDetailsImage("ability", NameAll.ABILITY_SLOT_MOVEMENT);
    }



    public void PopulateDetailsImage(string type, int slot)
    {
        if( detailsPanel.activeSelf)
        {
            detailsPanel.SetActive(false);
        }
        else
        {
            detailsPanel.SetActive(true);
            if( puFile != null)
            {
                int id;
                if( type.Equals("ability"))
                {
                    //Debug.Log("testing populate details image " + slot + " " + type);
                    if (slot == NameAll.ABILITY_SLOT_PRIMARY)
                    {
                        id = puFile.ClassId; //Debug.Log("testing populate details image " + id + type);
                    }
                    else if (slot == NameAll.ABILITY_SLOT_SECONDARY)
                    {
                        id = puFile.AbilitySecondaryCode;
                    }
                    else if (slot == NameAll.ABILITY_SLOT_REACTION)
                    {
                        id = puFile.AbilityReactionCode;
                    }
                    else if (slot == NameAll.ABILITY_SLOT_SUPPORT)
                    {
                        id = puFile.AbilitySupportCode;
                    }
                    else
                    {
                        id = puFile.AbilityMovementCode;
                    }
                    AbilityObject io = AbilityManager.Instance.GetAbilityObject(slot, id); //Debug.Log("" + io.GetAbilityName() + " " + io.GetClassId() + " " + io.GetDescription());
                    detailsTitle.text = io.AbilityName;
                    detailsDetails.text = io.Description;
                }
                else
                {
                    if( slot == NameAll.ITEM_SLOT_WEAPON)
                    {
                        id = puFile.ItemSlotWeapon;
                    }
                    else if (slot == NameAll.ITEM_SLOT_OFFHAND)
                    {
                        id = puFile.ItemSlotOffhand;
                    }
                    else if (slot == NameAll.ITEM_SLOT_HEAD)
                    {
                        id = puFile.ItemSlotHead;
                    }
                    else if (slot == NameAll.ITEM_SLOT_BODY)
                    {
                        id = puFile.ItemSlotBody;
                    }
                    else
                    {
                        id = puFile.ItemSlotAccessory;
                    }
                    ItemObject io = ItemManager.Instance.GetItemObjectById(id, slot);
                    detailsTitle.text = io.ItemName;
                    detailsDetails.text = ItemManager.Instance.GetDetails(io);
                }
            }

        }
    }

    void CloseDetailsPanel()
    {
        detailsPanel.SetActive(false);
    }
    
}
