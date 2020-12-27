using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISpellNameDetails : MonoBehaviour
{

    public Text spellName; public Text commandSet;
    public Text mod; public Text ctr;
    public Text MP; public Text baseHit;
    public Text baseQ; public Text doesDmg;
    public Text dmgType; public Text statType;
    public Text addStatus; public Text statusName;
    public Text pmType; public Text evasion;
    public Text range; public Text rangeZ;
    public Text effect; public Text effectZ;
    public Text elementType; public Text casterImmune;
    public Text alliesType; public Text dft;

    const string DidInfoButtonClick = "AbilitySelect.InfoButtonClick";

    void OnEnable()
    {
        this.AddObserver(OnInfoButtonClick, DidInfoButtonClick);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnInfoButtonClick, DidInfoButtonClick);
    }

    //details so far from UIAbilityScrollList
    void OnInfoButtonClick(object sender, object args)
    {
        //Debug.Log("received infobutton click");
        int z1 = (int)args;
        Populate(z1);
    }

    public void Populate(int index)
    {
        SpellName sn = SpellManager.Instance.GetSpellNameByIndex(index);
        spellName.text = sn.AbilityName; commandSet.text = CalcCode.SpellNameValueToString(sn, NameAll.SN_COMMAND_SET);
        mod.text = "Mod: "+ NameAll.GetModName(sn.Mod); ctr.text = "CTR: " + sn.CTR;
        MP.text = "MP: " + sn.MP; baseHit.text = "Base Hit: " + sn.BaseHit;
        baseQ.text = "Base Q: " + sn.BaseQ; doesDmg.text = "Does Dmg: " + NameAll.GetHitsStatString(sn.HitsStat);
        dmgType.text = "Dmg Type: " + NameAll.GetRemoveStatString(sn.RemoveStat,sn.HitsStat); statType.text = "Dmg Stat: " + NameAll.GetStatTypeString(sn.StatType);

        addStatus.text = "Adds Status: " + NameAll.GetHitsStatString(sn.AddsStatus); statusName.text = NameAll.GetStatusString(sn.StatusType);
        pmType.text = "Evasion Type: " + NameAll.GetPmTypeString(sn.PMType); evasion.text = "Evasion: " + NameAll.GetHitsStatString(sn.EvasionReflect);
        range.text = "Range: " + NameAll.GetRangeTypeString(sn.RangeXYMin, sn.RangeXYMax); rangeZ.text = "Range Z: " + NameAll.GetEffectZString(sn.RangeZ);

        effect.text = "Effect: " + sn.EffectXY; effectZ.text = "Effect Z: " + NameAll.GetEffectZString(sn.EffectZ);
        elementType.text = "Element: " + NameAll.GetElementalString(sn.ElementType); casterImmune.text = "Caster Immune: " + NameAll.GetHitsStatString(sn.CasterImmune);
        alliesType.text = "Allies/Enemies: " + NameAll.GetAlliesTypeString(sn.AlliesType); dft.text = "DFT: " + sn.DamageFormulaType;

    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    //    *raw name*	commandSet(meh)
    //Mod:	CTR:
    //MP:	Base Hit:
    //Base Q:	Does Dmg: (hits stat)
    //Dmg type(remove stat)  stat type
    //Adds Status(name of status if applicable)
    //pmType evasion/reflect
    //range: min to max range z
    //effect xy effect z
    //elementType caster immune
    //allies type dft

}
