using System.Collections;
using System.Collections.Generic;

public class InventoryItem
{
    public int value;
    public string name;
    public bool canBeDropped;
    public string description = "";
    public virtual bool equipable { get { return false; } }

    public void Set(int itemValue, string itemName, bool droppable, string itemDescription)
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

public class equipment : InventoryItem
{
    //TODO: add cursed equipment and non-removable equipment
    public override bool equipable { get { return true; } }
    public bool equipped { 
        get { 
            if (equippedPosition != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        } 
    }
    public List<DamageSystem.equipPosition> equipablePositions;
    public DamageSystem.equipPosition? equippedPosition = null;
    
}

public class WeaponItem : equipment
{
    public DamageSystem.damageType damageType;
    public float damageScale;
    public int baseDamage;
    public int reach;
    
    public WeaponItem(int goldValue, string itemName, bool droppable, DamageSystem.damageType weaponDamageType, float damageStatScaling, int weaponBaseDamage, int weaponReach)
    {
        value = goldValue;
        name = itemName;
        canBeDropped = droppable;
        damageType = weaponDamageType;
        damageScale = damageStatScaling;
        baseDamage = weaponBaseDamage;
        reach = weaponReach;
        equipablePositions = new List<DamageSystem.equipPosition>();
        equipablePositions.Add(DamageSystem.equipPosition.RightHand);
        equipablePositions.Add(DamageSystem.equipPosition.LeftHand);
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
        equipablePositions = new List<DamageSystem.equipPosition>();
        equipablePositions.Add(DamageSystem.equipPosition.RightHand);
        equipablePositions.Add(DamageSystem.equipPosition.LeftHand);
        return dupe;
    }
}


