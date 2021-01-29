﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldTile
{
    public worldTile(bool passable)
    {
        passable_ = passable;
    }
    bool passable_;
    public bool passable
    {
        get {return passable_;}
        set { passable_ = value; }
    }
}



public class Tileboard : MonoBehaviour
{
    public worldTile[,] tiles;
    public Tilescript[,] tilescripts;
    public GameObject[,] tileObjs;
    public int W = 20;
    public int H = 20;
    LoadAssetBundles assetBundleLoader;
    GameObject mainCamera;
    tileActor playerActor;

    public Color globalColor;
    bool pathfinding = false;
    float pathfindingDelay = 0.2f;
    float pathfindingTick = 0f;

    [SerializeField] LayerMask raycastMask;
    public float targetXrot;
    public float currentXrot;
    AstarPathfinding aStar;
    GameObject cameraTarget;

    bool initalLook = false;
    //anchored top left, 
    //x+ is right
    //y+ is down

    List<Tilescript> playerVisibleTiles;

    public enum direction 
    { 
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }

    void Start()
    {
        playerVisibleTiles = new List<Tilescript>();
        switch (Random.Range(0, 5))
        {
            case 0: globalColor = Color.red;
                break;
            case 1:
                globalColor = Color.blue;
                break;
            case 2:
                globalColor = Color.yellow;
                break;
            case 3:
                globalColor = Color.green;
                break;
            case 4:
                globalColor = Color.cyan;
                break;
        }
        aStar = GetComponent<AstarPathfinding>();
        assetBundleLoader = GetComponent<LoadAssetBundles>();
        InitalizeBlankTileboard(W, H);
        mainCamera = GameObject.Find("MainCamera");
        playerActor = CreateTileActor (Mathf.RoundToInt(W / 2), Mathf.RoundToInt(H / 2), assetBundleLoader.getSprite("mans"));
        mainCamera.transform.position = playerActor.gameObject.transform.position + new Vector3(0f, 0f, -5f);

        cameraTarget = new GameObject("cam Target");
        cameraTarget.transform.position = playerActor.gameObject.transform.position + Vector3.back * 5f;
        cameraTarget.transform.SetParent(playerActor.gameObject.transform);

        currentXrot = Mathf.Lerp(currentXrot, targetXrot, 0.01f);
        float t = currentXrot / -90.1f;
        cameraTarget.transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(-10, -1, t));
        playerActor.gameObject.transform.rotation = Quaternion.Euler(currentXrot, 0f, 0f);
        mainCamera.transform.position = cameraTarget.transform.position;
        mainCamera.transform.rotation = Quaternion.Euler(currentXrot, 0f, 0f);

        RotationTick();

        InventoryItem testItem = new InventoryItem();
        testItem.Set(0, "test item", true, "fat");

        InstantiateItem(playerActor.x, playerActor.y, testItem);
        
    }

    public void RotationTick()
    {
        currentXrot = Mathf.Lerp(currentXrot, targetXrot, 0.01f);
        float t = currentXrot / -90.1f;
        cameraTarget.transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(-10, -1, t));
        playerActor.gameObject.transform.rotation = Quaternion.Lerp(playerActor.gameObject.transform.rotation, Quaternion.Euler(currentXrot, 0f, 0f), 5f * Time.deltaTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTarget.transform.position, 3f * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, Quaternion.Euler(currentXrot, 0f, 0f), 5f * Time.deltaTime);
    }

    public tileActor CreateTileActor(int xPos, int yPos, Sprite image)
    {
        
        tileActor toReturn = new tileActor(xPos,yPos,image, Instantiate(assetBundleLoader.getPrefab("tileactor")),this,3,3,3,3,3);
        toReturn.tileActorScript = toReturn.gameObject.GetComponent<TileActorScript>();
        SpriteRenderer actorRenderer = toReturn.gameObject.GetComponent<SpriteRenderer>();
            actorRenderer.sprite = image;
        actorRenderer.color = globalColor;
        return toReturn;
    }

    public void InitalizeBlankTileboard(int width, int height)//for testing
    {
        Material wallMat_cracked = new Material(Shader.Find("Standard"));
        Material wallMat_cracked_light = new Material(Shader.Find("Standard"));
        Material wallMat_cracked_lighter = new Material(Shader.Find("Standard"));
        wallMat_cracked.mainTexture = assetBundleLoader.getTexture("wall_cracked");
        wallMat_cracked_light.mainTexture = assetBundleLoader.getTexture("wall_cracked_light");
        wallMat_cracked_lighter.mainTexture = assetBundleLoader.getTexture("wall_cracked_lighter");

        tiles = new worldTile[width,height];
        tileObjs = new GameObject[width, height];
        tilescripts = new Tilescript[width, height];

        for (int y = 0; y< height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                if (Random.Range(0f, 1f) > 0.75f && (x!=Mathf.RoundToInt(W / 2)&&y!= Mathf.RoundToInt(H / 2)))
                {//impassable wall
                    tiles[x, y] = new worldTile(false);
                    tileObjs[x, y] = Instantiate(assetBundleLoader.getPrefab("wall"));
                    
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            tileObjs[x, y].GetComponent<Renderer>().material = wallMat_cracked;
                            break;
                        case 1:
                            tileObjs[x, y].GetComponent<Renderer>().material = wallMat_cracked_light;
                            break;
                        case 2:
                            tileObjs[x, y].GetComponent<Renderer>().material = wallMat_cracked_lighter;
                            break;
                        default:
                            tileObjs[x, y].GetComponent<Renderer>().material = wallMat_cracked;
                            break;
                    }

                    tileObjs[x, y].transform.SetParent(transform);
                    tileObjs[x, y].transform.localPosition = new Vector3(x, y, -0.5f);
                    tileObjs[x, y].GetComponent<Renderer>().material.color = Color.black;
                    Tilescript thisTilescript = tileObjs[x, y].GetComponent<Tilescript>();
                    
                    
                    thisTilescript.Set(x, y, false);
                    tilescripts[x, y] = tileObjs[x, y].GetComponent<Tilescript>();

                }
                else
                {//passable
                    tiles[x, y] = new worldTile(true);
                    tileObjs[x, y] = Instantiate(assetBundleLoader.getPrefab("tileobject")); //Instantiate(tilePrefab);

                    Sprite tileSprite;
                    switch (Random.Range(0, 4))
                    {
                        case 0:
                            tileSprite = assetBundleLoader.getSprite("speckles");
                            break;
                        case 1:
                            tileSprite = assetBundleLoader.getSprite("speckles_alt1");
                            break;
                        case 2:
                            tileSprite = assetBundleLoader.getSprite("speckles_alt2");
                            break;
                        case 3:
                            tileSprite = assetBundleLoader.getSprite("speckles_alt3");
                            break;
                        default:
                            tileSprite = assetBundleLoader.getSprite("speckles");
                            break;
                    }

                    tileObjs[x, y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tileSprite;
                    tileObjs[x, y].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
                    tileObjs[x, y].transform.SetParent(transform);
                    tileObjs[x, y].transform.localPosition = new Vector3(x, y, 0f);

                    Tilescript thisTilescript = tileObjs[x, y].GetComponent<Tilescript>();
                    thisTilescript.Set(x, y, true);
                    tilescripts[x, y] = thisTilescript;
                }

                
            }
        }

        //enable pathfinding
        aStar.Init(this);
    }

    private void Update()
    {

        if (!initalLook)
        {
            initalLook = CalculateVisibility(5f);
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            targetXrot = Mathf.Clamp(targetXrot + Input.mouseScrollDelta.y * 5f, -90, 0f);
        }

        if (!pathfinding)
        {

            if (Input.GetKeyDown(KeyCode.J))
            {
                playerActor.PickupItemOnTile(0);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                playerActor.SetPosition(playerActor.x, playerActor.y + 1);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                playerActor.SetPosition(playerActor.x, playerActor.y - 1);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                playerActor.SetPosition(playerActor.x - 1, playerActor.y);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                playerActor.SetPosition(playerActor.x + 1, playerActor.y);
                CalculateVisibility(5f);
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                playerActor.MoveDirection(direction.SW);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                playerActor.MoveDirection(direction.S);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                playerActor.MoveDirection(direction.SE);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                playerActor.MoveDirection(direction.W);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                playerActor.MoveDirection(direction.E);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                playerActor.MoveDirection(direction.NW);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                playerActor.MoveDirection(direction.N);
                CalculateVisibility(5f);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                playerActor.MoveDirection(direction.NE);
                CalculateVisibility(5f);
            }
            //TODO: TICK ALL OTHER ACTORS HERE
        }
        else
        {//pathfinding
            if (pathfindingTick > pathfindingDelay)
            {
                pathfindingTick = 0;
                
                if (playerActor.pathfindingInstruction.Count < 1)
                {
                    pathfinding = false;
                }
                else
                {
                    playerActor.PathfindTick();
                    CalculateVisibility(5f);
                }
            }
            else
            {
                pathfindingTick += Time.deltaTime;
            }
        }

        RotationTick();
        
        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500f))
            {
                if (hit.transform!= null)
                {
                    Tilescript hitScript = hit.transform.gameObject.GetComponent<Tilescript>();
                    if (hitScript!= null && hitScript.passable)
                    {
                        aStar.UpdatePathfinder();
                        playerActor.pathfindingInstruction = aStar.fullPathfind(playerActor.x,playerActor.y,hitScript.x,hitScript.y);
                        pathfinding = true;
                        hitScript.Bounce();
                    }
                }
            }
        }

    }

    public void setTileColor(int x, int y, Color c)
    {
        if (tiles[x, y].passable)
        {
            tilescripts[x, y].thisSprite.color = c;
        }
        else
        {
            tilescripts[x, y].thisMat.color = c;
            tilescripts[x, y].thisMesh.enabled = true;
        }
    }

    bool CalculateVisibility(float sight)
    {
        List<RaycastHit> colliders;
        Vector3 center = tileObjs[playerActor.x, playerActor.y].transform.position;
        colliders = new List<RaycastHit>( Physics.CapsuleCastAll(center + Vector3.back, center + Vector3.forward,sight+.1f,Vector3.forward));
        if (colliders.Count < 1)
        {
            return false;
        }

        List<Tilescript> newVisibleTiles;
        newVisibleTiles = new List<Tilescript>();

        foreach (RaycastHit i in colliders)
        {
            Tilescript tile = i.transform.gameObject.GetComponent<Tilescript>();

            if (tile == null)
            {
                continue;
            }


            if (LineOfSightToTile(playerActor.x, playerActor.y, tile.x, tile.y,raycastMask))
            {
                newVisibleTiles.Add(tile);
                setTileColor(tile.x, tile.y, Color.white);
            }
        }

        for (int i = 0; i < playerVisibleTiles.Count; i++)
        {
            if (!newVisibleTiles.Contains(playerVisibleTiles[i]))
            {
                setTileColor(playerVisibleTiles[i].x, playerVisibleTiles[i].y, Color.grey);
            }
        }

        playerVisibleTiles = new List<Tilescript>(newVisibleTiles);

        return true;

    }

    public struct tileCord
    {
        public int x;
        public int y;
        public tileCord(int xCord, int yCord)
        {
            x = xCord;
            y = yCord;
        }
    }

    public bool LineOfSightToTile(int startx, int starty, int endx, int endy, int mask)
    {
        RaycastHit hit;
        Vector3 start = tileObjs[startx, starty].transform.position + Vector3.back * .5f;
        Vector3 end = tileObjs[endx, endy].transform.position + Vector3.back * .5f;
        

        if (Mathf.Abs(startx - endx) == Mathf.Abs(starty - endy))
        {
            int dx = endx - startx;
            int dy = endy - starty;
            Vector2 dir = new Vector2(0,0);
            if (dx>0 && dy > 0)
            {//NE
                dir = new Vector2(1, 1);
            }else if(dx < 0 && dy > 0)
            {//NW
                dir = new Vector2(-1, 1);
            }
            else if (dx > 0 && dy < 0)
            {//SE
                dir = new Vector2(1, -1);
            }
            else if (dx < 0 && dy < 0)
            {//SW
                dir = new Vector2(-1, -1);
            }
            
            for (int i = 0; i < Mathf.Abs(dx); i++)
            {
                int castTileX = Mathf.FloorToInt(startx + Mathf.Sign(dir.x) * i);
                int castTileY = Mathf.FloorToInt(starty + Mathf.Sign(dir.y) * i);

                if ((!tilescripts[Mathf.FloorToInt(castTileX + Mathf.Sign(dir.x)), castTileY].passable && !tilescripts[castTileX, Mathf.FloorToInt(castTileY + Mathf.Sign(dir.y))].passable) || !tilescripts[castTileX,castTileY].passable)
                {
                    return false;  
                }
            }
            return true;
        }


        Ray sight = new Ray(start,end-start);
        
        if ( !Physics.Raycast(sight, out hit, Vector3.Distance(start,end) ,mask))
        {
            Debug.DrawLine(start, end, Color.red, 20f);
            return true;
        }
        else
        {
            if (hit.transform.gameObject == tileObjs[endx, endy])
            {
                Debug.DrawLine(start, end, Color.yellow, 20f);
                return true;
            }
            Debug.DrawLine(start, end, Color.green, 2f);
            return false;
        }
    }

    public GameObject InstantiateItem(int x, int y, InventoryItem item)
    {
        GameObject spawn = Instantiate( assetBundleLoader.getPrefab("TileItem"));
        TileItem spawnedItem = spawn.GetComponent<TileItem>();
        spawnedItem.set(item, x, y, this);
        spawn.transform.SetParent(this.transform);
        spawn.transform.localPosition = new Vector3((float)x, (float)y, -0.5f);
        tilescripts[x, y].itemsOnTile.Add(spawnedItem);
        return spawn;
    }
}