using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatUITarget : MonoBehaviour
{
    //public Image sexImage;
    public Image genderImage;
    public Text classText;
    public Text hpText;
    public Text mpText;
    public Text paText;
    public Text braveText;

    Color neutralColor = new Color32(126,209,232,255);
    Color team1Color = new Color32(236, 142, 47, 255);
    Color team2Color = new Color32(129, 77, 197, 255);

    int actorPanelId = NameAll.NULL_INT; //for listening to notifications

    public void SetActor(PlayerUnit pu )
    {
		if ( gameObject.activeSelf == false )
            Open(); //Debug.Log("in set actor, team is " + pu.TeamId);

        actorPanelId = pu.TurnOrder;
        if (pu.TeamId == NameAll.TEAM_ID_GREEN)
        {
            //var rend = this.gameObject.GetComponent<Renderer>();
            //rend.material.mainTexture = Resources.Load("menu_team_1") as Texture;
            //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_1");
            gameObject.GetComponent<Image>().color = team1Color;
        }
        else if (pu.TeamId == NameAll.TEAM_ID_RED)
        {
            //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_team_2");
            gameObject.GetComponent<Image>().color = team2Color;
        }
        else
        {
            //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
            gameObject.GetComponent<Image>().color = neutralColor;
        }

        string zString = "";
        if( pu.Sex.Equals("Male"))
        {
            zString = "male_" + (pu.ZodiacInt + 1);
        }
        else
        {
            zString = "female_" + (pu.ZodiacInt + 1);
        }
        genderImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Zodiac/"+zString); //Debug.Log("Sprites/Zodiac/" + zString);
        //genderImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(zString);

        string shortName = pu.UnitName.Length <= 5 ? pu.UnitName : pu.UnitName.Substring(0, 5);
        classText.text = "" + pu.TurnOrder + " " + AbilityManager.Instance.GetAbilityName(NameAll.ABILITY_SLOT_PRIMARY, pu.ClassId) + " " + shortName;
        hpText.text = "HP: " + pu.StatTotalLife + "/" + pu.StatTotalMaxLife;
        mpText.text = "MP: " + pu.StatTotalMP + "/" + pu.StatTotalMaxMP + " TP: " + pu.CT + "/100";
        paText.text = pu.StatTotalPA + "/" + pu.StatTotalMA + "/" + pu.StatTotalAgi + " | " + pu.StatTotalSpeed + "/" + pu.StatTotalMove + "/" + pu.StatTotalJump
            + " | " + pu.GetWeaponPower(false, true) + "/" + pu.Level + "/" + pu.Dir; 
        braveText.text = pu.StatTotalBrave + "/" + pu.StatTotalFaith + "/" + pu.StatTotalCunning 
            + " | " + pu.StatTotalCEvade + "/" + pu.StatItemOffhandPEvade + "/" + pu.StatItemOffhandMEvade 
            + "/" + pu.StatItemAccessoryMEvade + "/" + pu.StatItemAccessoryPEvade + "/" + pu.StatItemWEvade;

    }

    public void SetTargetPreview( Board board, Point pos)
    {
        //Debug.Log("getting pos for " + pos.x + " " + pos.y);
        SetTargetPreview(board.GetTile(pos)); 
    }

    public void SetTargetPreview(Tile t)
    {
        //if( t == null)
        //{
        //    Debug.Log("tile is null");
        //}
        //Debug.Log("target unit Id is " + t.UnitId);
        if( t.UnitId == NameAll.NULL_UNIT_ID)
        {
            SetTile(t);
        }
        else
        {
            SetActor(PlayerManager.Instance.GetPlayerUnit(t.UnitId));
        }

    }

    public void SetTargetPreview(PlayerUnit pu)
    {
        SetActor(pu);
    }

    //in walk around mode, can set the targer to see info or see actor menu
    public PlayerUnit SetWalkAroundTargetPreview(Board board, Point pos)
    {
        //Debug.Log("getting pos for " + pos.x + " " + pos.y);
        return SetWalkAroundTargetPreview(board.GetTile(pos));
    }

    public PlayerUnit SetWalkAroundTargetPreview(Tile t)
    {
        //if( t == null)
        //{
        //    Debug.Log("tile is null");
        //}
        //Debug.Log(" in setwalkaroundtargetpreview 0 target unit Id is " + t.UnitId);
        if (t.UnitId == NameAll.NULL_UNIT_ID)
        {
			//Debug.Log(" in setwalkaroundtargetpreview 1 target unit Id is " + t.UnitId);
			SetTile(t);
            return null;
        }
        else
        {
			//Debug.Log(" in setwalkaroundtargetpreview 2 target unit Id is " + t.UnitId);
			var pu = PlayerManager.Instance.GetPlayerUnit(t.UnitId);
            SetActor(pu);
            return pu;
        }

    }


    void SetTile(Tile t)
    {
        Open();
        gameObject.GetComponent<Image>().color = neutralColor;
        //gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu_neutral");
        genderImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("grass_terrain");
        classText.text = "Map Tile";
        hpText.text = "X: " + t.pos.x + " Y: " + t.pos.y;
        mpText.text = "Height: " + t.height;
        paText.text = "Terrain: " + "grass";
        braveText.text = "Unit Id: " + t.UnitId;
    }


    public void SetHitPreview(string spellName, string hit, string effect, string addStatus, string reaction, bool isImage = false)
    {
        Open();
        gameObject.GetComponent<Image>().color = neutralColor;
        //genderImage.SetActive(isImage); //need to access the main game object
        classText.text = spellName;
        hpText.text = "Hit %: " + hit;
        mpText.text = "Effect: " + effect;
        paText.text = addStatus;
        braveText.text = "Reaction: " + reaction;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        //EnableObservers();
    }

    public void Close()
    {
        actorPanelId = NameAll.NULL_INT;
        //DisableObservers();
        gameObject.SetActive(false);
    }

    //was used to update the actor stat panel. due to errors in multiplayer when quickly going between unit turns, this was disabled 

    //const string ActorStatChangeNotification = "CombatUITarget.ActorStatChangeNotification";

    //void EnableObservers()
    //{
    //    this.AddObserver(OnActorStatChange, ActorStatChangeNotification);
    //}

    //void DisableObservers()
    //{
    //    this.RemoveObserver(OnActorStatChange, ActorStatChangeNotification);
    //}

    ////if actor receives some stat dmg, update the panel
    //void OnActorStatChange(object sender, object args)
    //{
    //    PlayerUnit pu = (PlayerUnit)args;
    //    if( pu != null && pu.TurnOrder == actorPanelId)
    //    {
    //        SetActor(pu);
    //    }
    //}


}
