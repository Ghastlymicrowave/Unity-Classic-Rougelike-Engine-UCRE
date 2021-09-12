using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileActorScript : MonoBehaviour
{
    public Vector3 targetPosition = Vector3.zero;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    public void ForceMoveToTarget()
    {
        transform.position = targetPosition;
    }

    private void Update()
    {
        if (Vector3.Distance(targetPosition, transform.position) > 0.01)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 8f * Time.deltaTime);
        }
    }
}

public class tileActor
{
    public string name = "default dude";
    public GameObject refrenceObject;
    public TileActorScript tileActorScript;
    public Tileboard tileboardReference;
    public Sprite image;
    public int xCord;
    public int yCord;

    public List<Tileboard.direction> pathfindingInstruction;
    public List<InventoryItem> inventory;
    public equipment[] equips =  new equipment[DamageSystem.equipPosition.GetNames(typeof(DamageSystem.equipPosition)).Length];
    
    //stats
    public int maxHP
    {
        get { return DamageSystem.hitpointsPerHP * stat_HP; }
    }

    public int currentHP;
  

    public int stat_HP;
    public int stat_str;//heavy weapons, bashing weapons
    //Skills:
    public int stat_dex;//light weaons, piercing weapons
    //Skills:
    //slashing/medium weapons use both
    public int stat_int;//magic damage, skillpoints to unlock skills/feats for stats
    public int stat_wis;//magic accuracy and item usage proficency

    public tileActor() { }//don't remove, C# is weird
    public tileActor(int xPos, int yPos, Sprite sprite, GameObject gameObject, Tileboard tileboard, int HP, int str, int dex, int intel, int wis)
    {
        image = sprite;
        refrenceObject = gameObject;
        tileboardReference = tileboard;
        SetPosition(xPos, yPos);
        tileActorScript = gameObject.GetComponent<TileActorScript>();
        
        stat_HP = HP;
        stat_str = str;
        stat_dex = dex;
        stat_int = intel;
        stat_wis = wis;

        currentHP = maxHP;

        pathfindingInstruction = new List<Tileboard.direction>();
        inventory = new List<InventoryItem>();

        inventory.Add(new WeaponItem(0, "Fists", false, DamageSystem.damageType.blunt, 1.2f, 5, 1));
        EquipItem(inventory[0] as WeaponItem, DamageSystem.equipPosition.RightHand);
    }

    public void SetPosition(int newXCord, int newYCord)
    {
        newXCord = Mathf.Clamp(newXCord, 0, tileboardReference.W - 1);
        newYCord = Mathf.Clamp(newYCord, 0, tileboardReference.H - 1);
        if (tileboardReference.tilescripts[newXCord, newYCord].passable)
        {
            xCord = newXCord;
            yCord = newYCord;
            ReloadPosition();
        }
        else
        {
            Debug.Log("tried moving onto an impassable tile");
        }
    }

    public int PositionOnTileboardValid(int x, int y)
    {
        //0 is not valid
        //1 means you can move
        //2 means there is an actor
        if ((x > -1 && x < tileboardReference.W && y > -1 && y < tileboardReference.H) && tileboardReference.tilescripts[x,y].passable)
        {
            if (tileboardReference.tilescripts[x, y].actorOnTile != null)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 0;
        }
    }

    void ReloadPosition()
    {
        TileActorScript tile = gameObject.GetComponent<TileActorScript>();
        tile.ForceMoveToTarget();
        tile.targetPosition = new Vector3(xCord, yCord, -0.5f);
    }

    public void PathfindTick()
    {
        if (pathfindingInstruction.Count > 0)
        {
            MoveDirection(pathfindingInstruction[0]);
            pathfindingInstruction.RemoveAt(0);
        }
    }

    public int MoveDirection(Tileboard.direction direction)
    {
        //returns 1 if could sucessfully move
        //returns 0 if could not move
        //returns 2 if there was an actor blocking movement
        tileboardReference.tilescripts[x, y].actorOnTile = null;

        switch (direction)
        {
            case Tileboard.direction.N:
                if (PositionOnTileboardValid(xCord, yCord + 1) == 1)
                {
                    yCord += 1;
                    ReloadPosition();
                }
                tileboardReference.tilescripts[x, y].actorOnTile = this;
                return PositionOnTileboardValid(xCord, yCord + 1);
            case Tileboard.direction.E:
                if (PositionOnTileboardValid(xCord + 1, yCord) == 1)
                {
                    xCord += 1;
                    ReloadPosition();
                }
                tileboardReference.tilescripts[x, y].actorOnTile = this;
                return PositionOnTileboardValid(xCord + 1, yCord);
            case Tileboard.direction.S:
                if (PositionOnTileboardValid(xCord, yCord - 1) == 1)
                {
                    yCord -= 1;
                    ReloadPosition();
                }
                tileboardReference.tilescripts[x, y].actorOnTile = this;
                return PositionOnTileboardValid(xCord, yCord - 1);
            case Tileboard.direction.W:
                if (PositionOnTileboardValid(xCord - 1, yCord) == 1)
                {
                    xCord -= 1;
                    ReloadPosition();
                }
                tileboardReference.tilescripts[x, y].actorOnTile = this;
                return PositionOnTileboardValid(xCord - 1, yCord);
            case Tileboard.direction.NE:
                if (PositionOnTileboardValid(xCord + 1, yCord + 1) == 1 &&
                    (PositionOnTileboardValid(xCord, yCord + 1) ==1 || PositionOnTileboardValid(xCord + 1, yCord) ==1))
                {
                    yCord += 1;
                    xCord += 1;
                    ReloadPosition();
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    return 1;
                }
                else
                {
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    if ((PositionOnTileboardValid(xCord + 1, yCord + 1) == 2 ||
                    (PositionOnTileboardValid(xCord, yCord + 1) == 2 || PositionOnTileboardValid(xCord + 1, yCord) == 2)))
                    { return 2; }
                    else { return 0; }
                }
            case Tileboard.direction.SE:
                if (PositionOnTileboardValid(xCord + 1, yCord - 1) == 1 && 
                    (PositionOnTileboardValid(xCord, yCord - 1) ==1 || PositionOnTileboardValid(xCord + 1, yCord) ==1))
                {
                    yCord -= 1;
                    xCord += 1;
                    ReloadPosition();
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    return 1;
                }
                else
                {
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    if (PositionOnTileboardValid(xCord + 1, yCord - 1) == 2 ||
                    (PositionOnTileboardValid(xCord, yCord - 1) == 2 || PositionOnTileboardValid(xCord + 1, yCord) == 2))
                    { return 2; }
                    else { return 0; }
                }
            case Tileboard.direction.SW:
                if (PositionOnTileboardValid(xCord - 1, yCord - 1) == 1 &&
                    (PositionOnTileboardValid(xCord, yCord - 1) ==1 || PositionOnTileboardValid(xCord - 1, yCord) ==1))
                {
                    yCord -= 1;
                    xCord -= 1;
                    ReloadPosition();
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    return 1;
                }
                else
                {
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    if (PositionOnTileboardValid(xCord - 1, yCord - 1) == 2 ||
                    (PositionOnTileboardValid(xCord, yCord - 1) == 2 || PositionOnTileboardValid(xCord - 1, yCord) == 2))
                    { return 2; }
                    else { return 0; }
                }
            case Tileboard.direction.NW:
                if (PositionOnTileboardValid(xCord - 1, yCord + 1) == 1 && 
                    (PositionOnTileboardValid(xCord, yCord + 1) ==1 || PositionOnTileboardValid(xCord - 1, yCord) ==1))
                {
                    yCord += 1;
                    xCord -= 1;
                    ReloadPosition();
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    return 1;
                }
                else
                {
                    tileboardReference.tilescripts[x, y].actorOnTile = this;
                    if (PositionOnTileboardValid(xCord - 1, yCord + 1) == 2 ||
                    (PositionOnTileboardValid(xCord, yCord + 1) == 2 || PositionOnTileboardValid(xCord - 1, yCord) == 2))
                    { return 2; }
                    else {return 0; }
                }
            default:
                tileboardReference.tilescripts[x, y].actorOnTile = this; return 0;
        }
        
    }

    public void AttemptAttackOnTile(int targX, int targY, WeaponItem weapon)
    {
        int distX = Mathf.Abs(targX - x);
        int distY = Mathf.Abs(targY - y);
        int tileDistance;
        if (distX <= distY)
        {
            tileDistance = distX;
        }
        else
        {
            tileDistance = distY;
        }

        if (tileDistance < weapon.reach)
        {
            Debug.LogWarning("Attempting to attack with not enough reach,\n" +
                "attacker: " + this.name + " target: " + tileboardReference.tilescripts[targX, targY].actorOnTile.name +
                "distance: " + tileDistance.ToString() + " current reach: " + weapon.reach.ToString());
        }
        else
        {
            //TODO: CALCULATE CHANCE TO HIT
            //TODO: ADD WEAPON EFFECTS
            //TODO: ADD WARNING IF FRIENDLY
            tileboardReference.tilescripts[targX, targY].actorOnTile.ReceiveDamage(DamageSystem.CalcDamage(weapon, this));
        }

    }

    public void ReceiveDamage(DamageSystem.damageContainer inputContainer)
    {
        //TODO: ADD RESISTANCES TO DAMAGE TYPES?
        currentHP -= inputContainer.damage;
        if (currentHP == 0)
        {
            Killed();
        }
    }

    public void PickupItemOnTile(int index)
    {
        if (tileboardReference.tilescripts[x, y].itemsOnTile == null)
        {
            Debug.Log("nothing to pick up");
            return;
        }
        List<InventoryItem> items = tileboardReference.tilescripts[x, y].itemsOnTile.items;
        if (items.Count > 0 && items.Count > index)
        {
            inventory.Add(items[index].ItemClone());

            if (items.Count == 1)
            {
                GameObject toRemove = tileboardReference.tilescripts[x, y].itemsOnTile.gameObject;
                tileboardReference.tilescripts[x, y].itemsOnTile.items.RemoveAt(index);
                GameObject.Destroy(toRemove);
            }
            else
            {
                tileboardReference.tilescripts[x, y].itemsOnTile.items.RemoveAt(index);
            }

           
        }
    }

    public void DropItemOnTile(int index)
    {
        if (index >= inventory.Count)
        {
            Debug.Log("nothing to drop");
            return;
        }
        if (!inventory[index].canBeDropped)
        {
            Debug.Log("can't drop " + inventory[index].name);
            return;
        }
        tileboardReference.InstantiateItem(x, y, inventory[index]);
        inventory.RemoveAt(index);
    }

    public void Killed()
    {
        //TODO: do something and remove
    }

    public GameObject gameObject
    {
        get { return refrenceObject; }
    }

    public int x
    {
        get { return xCord; }
    }
    public int y
    {
        get { return yCord; }
    }

    public bool CheckIfCanEquip(equipment item, DamageSystem.equipPosition equipPos)
    {
        if (item.equipablePositions.Contains(equipPos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EquipItem(equipment item,DamageSystem.equipPosition equipPos)
    {
        if (!CheckIfCanEquip(item, equipPos)) { Debug.LogError("cannot equip item "+item.name+" in slot "+equipPos.ToString()); return; }
        int equipSlot = (int)equipPos;
        if (equips[equipSlot] == null)
        {//no equipment in slot
            equips[equipSlot] = item;
            item.equippedPosition = equipPos;
        }
        else
        {//replace item
            Debug.Log(equips[equipSlot].ToString());
            equips[equipSlot].equippedPosition = null;
            equips[equipSlot] = item;
            item.equippedPosition = equipPos;
        }
    }
}


public class autoTileActor : tileActor
{
    huntStatusContainer huntStatus;

    public autoTileActor(int xPos, int yPos, Sprite sprite, GameObject gameObject, Tileboard tileboard, int HP, int str, int dex, int intel, int wis)
    {
        image = sprite;
        refrenceObject = gameObject;
        tileboardReference = tileboard;
        SetPosition(xPos, yPos);
        tileActorScript = gameObject.GetComponent<TileActorScript>();

        stat_HP = HP;
        stat_str = str;
        stat_dex = dex;
        stat_int = intel;
        stat_wis = wis;

        currentHP = maxHP;

        pathfindingInstruction = new List<Tileboard.direction>();
        inventory = new List<InventoryItem>();

        inventory.Add(new WeaponItem(0, "Fists", false, DamageSystem.damageType.blunt, 1.2f, 5, 1));

        WeaponItem equippedWeapon = inventory[0] as WeaponItem;
    }

    public enum ControlMode
    {
        manual,
        hunt
    }

    public struct huntStatusContainer
    {
        public TileActorScript huntedTarget;
        public enum huntStatus
        {
            wander,
            chase,
            search,
        }
        public huntStatus status;
        public int timer;
        public int wanderTimerMax;
        public int chaseTimerMax;
        public int searchTimerMax;
        public huntStatusContainer(TileActorScript target, int startingtime, int wandertime, int chasetime, int searchtime, huntStatus currentstatus)
        {
            huntedTarget = target;
            status = currentstatus;
            timer = startingtime;
            wanderTimerMax = wandertime;
            chaseTimerMax = chasetime;
            searchTimerMax = searchtime;
        }
    }
}