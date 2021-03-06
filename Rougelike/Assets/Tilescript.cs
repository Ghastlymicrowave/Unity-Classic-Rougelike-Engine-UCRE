﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Tilescript : MonoBehaviour
{
    public int x;
    public int y;
    public bool passable;
    Vector3 originalPos;
    Animator anim;
    public Material thisMat;
    public SpriteRenderer thisSprite;
    public MeshRenderer thisMesh;
    public tileActor actorOnTile;
    public TileItem itemsOnTile;
    public enum visionState
    {
        unknown,
        hidden,
        visible
    }
    public visionState vison = visionState.unknown;
    public void Set(int xPos, int yPos, bool isPassable)
    {
        x = xPos;
        y = yPos;
        passable = isPassable;
        originalPos = transform.position;
        if (passable)
        {
            anim = transform.GetChild(0).GetComponent<Animator>();
            thisSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        else
        {
            thisMat = GetComponent<Renderer>().material;
            thisMesh = GetComponent<MeshRenderer>();
            thisMesh.enabled = false;
        }
        

    }

    public void Bounce()
    {
        anim.Play("TileBounce",0);
    }
}
