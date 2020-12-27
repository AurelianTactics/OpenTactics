using UnityEngine;
using System.Collections;

public abstract class CombatBaseAbilityPicker : MonoBehaviour
{
	#region Fields
	//protected Unit owner;
	//protected AbilityCatalog ac;
	#endregion

	#region MonoBehaviour
	//void Start ()
	//{
	//	owner = GetComponentInParent<Unit>();
	//	ac = owner.GetComponentInChildren<AbilityCatalog>();
	//}
	#endregion

	#region Public
	//public abstract void Pick (CombatPlanOfAttack combatPlan, PlayerUnit pu);
    #endregion

    #region Protected
    protected SpellName Find()
    {
        //for (int i = 0; i < ac.transform.childCount; ++i)
        //{
        //    Transform category = ac.transform.GetChild(i);
        //    Transform child = category.FindChild(abilityName);
        //    if (child != null)
        //        return child.GetComponent<Ability>();
        //}
        return null;
    }

    protected SpellName Default (PlayerUnit pu)
	{
        //return owner.GetComponentInChildren<Ability>();
        //Debug.Log("weapon type, class id" + pu.ItemSlotWeapon + "," + pu.ClassId);
        return SpellManager.Instance.GetSpellAttackByWeaponId(pu.ItemSlotWeapon, pu.ClassId);
	}
	#endregion
}