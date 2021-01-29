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

    public enum damageType{
        slashing,
        blunt,
        piercing,
        magical
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


public class InventoryItem 
{
    public int value;
    public string name;
    public bool canBeDropped;
    public string description = "";

    public virtual string itemType
    {
        get { return "none"; }
    }

    public void Set (int itemValue, string itemName, bool droppable, string itemDescription)
    {
        value = itemValue;
        name = itemName;
        canBeDropped = droppable;
        description = itemDescription;
    }

    public virtual InventoryItem ItemClone()
    {
        //create a clone
        InventoryItem dupe = (InventoryItem)this.MemberwiseClone();
        dupe.value = value;
        dupe.name = string.Copy(name);
        dupe.canBeDropped = canBeDropped;
        return dupe;
    }
}

public class ConsumableItem : InventoryItem
{
    public enum consumableType
    {
        duration,
        instant
    }

    public consumableType consumeType;

    
}

public class ConsumableEffectItem : ConsumableItem//lacking a clone
{
    DamageSystem.effect effect;
    DamageSystem.effectTickMode tickMode;
    int initalIntensity;
    int duration;
    int endingIntensity;

    public override string itemType
    {
        get { return "ConsumableEffectItem"; }
    }

    public ConsumableEffectItem(int goldValue, string itemName, bool droppable, DamageSystem.effect consumableEffect, DamageSystem.effectTickMode effectTickMode, int effectIntensity, int effectDuration)
    {
        consumeType = ConsumableItem.consumableType.duration;
        value = goldValue;
        name = itemName;
        canBeDropped = droppable;
        effect = consumableEffect;
        tickMode = effectTickMode;
        initalIntensity = effectIntensity;
        duration = effectDuration;
    }

    public ConsumableEffectItem(int goldValue, string itemName, bool droppable, DamageSystem.effect consumableEffect, DamageSystem.effectTickMode effectTickMode, int effectIntensity, int effectIntensityEnd, int effectDuration)
    {
        consumeType = ConsumableItem.consumableType.duration;
        value = goldValue;
        name = itemName;
        canBeDropped = droppable;
        effect = consumableEffect;
        tickMode = effectTickMode;
        initalIntensity = effectIntensity;
        endingIntensity = effectIntensityEnd;
        duration = effectDuration;
    }
}

public class InstantEffectItem : ConsumableItem //lacking a clone
{
    public DamageSystem.instantEffect effect;
    public int intensity;

    public override string itemType
    {
        get { return "InstantEffectItem"; }
    }

    public InstantEffectItem(int goldValue, string itemName, bool droppable, DamageSystem.instantEffect instantEffect, int effectIntensity)
    {
        consumeType = ConsumableItem.consumableType.instant;
        value = goldValue;
        name = itemName;
        canBeDropped = droppable;
        effect = instantEffect;
        intensity = effectIntensity;
    }

    
}

public class WeaponItem : InventoryItem 
{
    public DamageSystem.damageType damageType;
    public float damageScale;
    public int baseDamage;
    public int reach;

    public override string itemType
    {
        get { return "WeaponItem"; }
    }

    public WeaponItem(int goldValue, string itemName, bool droppable, DamageSystem.damageType weaponDamageType, float damageStatScaling, int weaponBaseDamage, int weaponReach)
    {
        value = goldValue;
        name = itemName;
        canBeDropped = droppable;
        damageType = weaponDamageType;
        damageScale = damageStatScaling;
        baseDamage = weaponBaseDamage;
        reach = weaponReach;
    }

    public override InventoryItem ItemClone()
    {
        WeaponItem dupe = (WeaponItem)this.MemberwiseClone();
        dupe.value = value;
        dupe.name = string.Copy(name);
        dupe.canBeDropped = canBeDropped;
        dupe.damageType = (DamageSystem.damageType)((int)damageType);
        dupe.damageScale = damageScale;
        dupe.baseDamage = baseDamage;
        dupe.reach = reach;

        return dupe;
    }
}

