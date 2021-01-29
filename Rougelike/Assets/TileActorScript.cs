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
    string name = "default dude";
    GameObject refrenceObject;
    public TileActorScript tileActorScript;
    Tileboard tileboardReference;
    Sprite image;
    int xCord;
    int yCord;

    public List<Tileboard.direction> pathfindingInstruction;
    public List<InventoryItem> inventory;
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

        WeaponItem equippedWeapon = inventory[0] as WeaponItem;
    }

    public void SetPosition(int newXCord, int newYCord)
    {
        newXCord = Mathf.Clamp(newXCord, 0, tileboardReference.W - 1);
        newYCord = Mathf.Clamp(newYCord, 0, tileboardReference.H - 1);
        if (tileboardReference.tiles[newXCord, newYCord].passable)
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

    public bool PositionOnTileboardValid(int x, int y)
    {
        return (x > -1 && x < tileboardReference.W && y > -1 && y < tileboardReference.H);
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

    public void MoveDirection(Tileboard.direction direction)
    {
        tileboardReference.tilescripts[x, y].actorOnTile = null;

        switch (direction)
        {
            case Tileboard.direction.N:
                if (PositionOnTileboardValid(xCord, yCord + 1) && tileboardReference.tiles[xCord, yCord + 1].passable)
                {
                    yCord += 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.E:
                if (PositionOnTileboardValid(xCord + 1, yCord) && tileboardReference.tiles[xCord + 1, yCord].passable)
                {
                    xCord += 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.S:
                if (PositionOnTileboardValid(xCord, yCord - 1) && tileboardReference.tiles[xCord, yCord - 1].passable)
                {
                    yCord -= 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.W:
                if (PositionOnTileboardValid(xCord - 1, yCord) && tileboardReference.tiles[xCord - 1, yCord].passable)
                {
                    xCord -= 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.NE:
                if (PositionOnTileboardValid(xCord + 1, yCord + 1) && tileboardReference.tiles[xCord + 1, yCord + 1].passable &&
                    (tileboardReference.tiles[xCord, yCord + 1].passable || tileboardReference.tiles[xCord + 1, yCord].passable))

                {
                    yCord += 1;
                    xCord += 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.SE:
                if (PositionOnTileboardValid(xCord + 1, yCord - 1) && tileboardReference.tiles[xCord + 1, yCord - 1].passable &&
                    (tileboardReference.tiles[xCord, yCord - 1].passable || tileboardReference.tiles[xCord + 1, yCord].passable)
                     )
                {
                    yCord -= 1;
                    xCord += 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.SW:
                if (PositionOnTileboardValid(xCord - 1, yCord - 1) && tileboardReference.tiles[xCord - 1, yCord - 1].passable &&
                    (tileboardReference.tiles[xCord, yCord - 1].passable || tileboardReference.tiles[xCord - 1, yCord].passable)
                     )
                {
                    yCord -= 1;
                    xCord -= 1;
                    ReloadPosition();
                }
                break;
            case Tileboard.direction.NW:
                if (PositionOnTileboardValid(xCord - 1, yCord + 1) && tileboardReference.tiles[xCord - 1, yCord + 1].passable &&
                    (tileboardReference.tiles[xCord, yCord + 1].passable || tileboardReference.tiles[xCord - 1, yCord].passable)
                     )
                {
                    yCord += 1;
                    xCord -= 1;
                    ReloadPosition();
                }
                break;
        }

        tileboardReference.tilescripts[x, y].actorOnTile = this;
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
        List<TileItem> items = tileboardReference.tilescripts[x, y].itemsOnTile;
        if (items.Count > 0 && items.Count > index)
        {
            inventory.Add(items[index].item.ItemClone());
            GameObject toRemove = tileboardReference.tilescripts[x, y].itemsOnTile[index].gameObject;
            tileboardReference.tilescripts[x, y].itemsOnTile.RemoveAt(index);
            GameObject.Destroy(toRemove);
        }
    }

    public void Killed()
    {
        //do something
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
}