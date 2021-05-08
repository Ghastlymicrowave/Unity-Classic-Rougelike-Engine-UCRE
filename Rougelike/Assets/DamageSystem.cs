using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem 
{
    public const int hitpointsPerHP = 10;

    public enum effect
    {
        speed,
        regen,
        poison,
        fire,
        str_boost,
        dex_boost,
        int_boost,
        wis_boost,
        tempHP
    }

    public enum instantEffect
    {
        health,
        damage,

    }
    
    public enum effectTickMode
    {
        increase,
        decrease,
        constant
    }

    public enum damageType
    {
        slashing,
        blunt,
        piercing,
        magical
    }

    public enum equipPosition
    {
        RightHand,
        LeftHand,
        Head,
        Body,
        Legs,
        Feet
    }

    public struct damageContainer
    {
        public int damage;
        public damageType damageType;
        public damageContainer(int dmg, damageType type){
            damage = dmg;
            damageType = type;
        }
    }

    public static damageContainer CalcDamage(WeaponItem weapon, tileActor user)
    {
        int baseDamage = weapon.baseDamage;
        float damageScale = weapon.damageScale;
        damageType type = weapon.damageType;
        int finalDamage;
        switch (type)
        {
            case damageType.blunt:
                finalDamage= baseDamage + Mathf.RoundToInt( user.stat_str * damageScale);break;
            case damageType.piercing:
                finalDamage= baseDamage + Mathf.RoundToInt(user.stat_dex * damageScale); break;
            case damageType.slashing:
                finalDamage= baseDamage + Mathf.RoundToInt(user.stat_dex + user.stat_str * damageScale); break;
            case damageType.magical:
                finalDamage= baseDamage + Mathf.RoundToInt(user.stat_int * damageScale); break;
            default: return new damageContainer(0, damageType.slashing);
        }
        return new damageContainer(finalDamage, type);
    }
}



