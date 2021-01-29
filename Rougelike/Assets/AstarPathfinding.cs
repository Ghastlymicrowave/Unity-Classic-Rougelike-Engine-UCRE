using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathfinding : MonoBehaviour
{
    Tileboard tileboardReference;
    tileCord[,] tiles;

    public void Init(Tileboard tileboard)
    {
        tileboardReference = tileboard;
        UpdatePathfinder();
    }

    public class tileCord
    {
        public int x = 0;
        public int y = 0;
        public bool passable = false;
        public int gCost = 0;
        public int hCost = 0;
        public tileCord parent;

        public tileCord(int xPos, int yPos, bool canBePassed)
        {
            x = xPos;
            y = yPos;
            passable = canBePassed;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
    }

    public void UpdatePathfinder()
    {
        tiles = new tileCord[tileboardReference.W, tileboardReference.H];
        for (int x = 0; x < tileboardReference.W; x++)
        {
            for (int y = 0; y < tileboardReference.H; y++)
            {
                worldTile thisTile = tileboardReference.tiles[x, y];
                tiles[x,y] = new tileCord(x, y, thisTile.passable);
            }
        }
    }

    public List<tileCord> GetNeighbors(tileCord tilecord)
    {
        List<tileCord> neighbors = new List<tileCord>();
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = tilecord.x + x;
                int checkY = tilecord.y + y;

                if (checkX >= 0 && checkX < tileboardReference.W && checkY >= 0 && checkY < tileboardReference.H)
                {
                    neighbors.Add(tiles[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    public List<Tileboard.direction> fullPathfind(int startingX, int startingY, int targetX, int targetY)
    {
        List<tileCord> open = new List<tileCord>();
        HashSet<tileCord> closed = new HashSet<tileCord>();
        tileCord start = tiles[startingX, startingY];
        open.Add(start);
        tileCord target = tiles[targetX, targetY];

        while (open.Count > 0)
        {
            tileCord current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost || (open[i].fCost == current.fCost && open[i].hCost < current.hCost))
                {
                    current = open[i];
                }
            }
            open.Remove(current);
            closed.Add(current);

            if (current == target)
            {
                return Retrace(start, target);
            }

            foreach(tileCord neighbor in GetNeighbors(current))
            {
                if (!neighbor.passable || closed.Contains(neighbor)){
                    continue;
                }

                int x = neighbor.x - current.x;
                int y = neighbor.y - current.y;
                
                if (x != 0 && y != 0)
                {
                    if (x == 1 && y == 1)//NE
                    {
                        if (!tiles[current.x, current.y + 1].passable
                        && !tiles[current.x + 1, current.y].passable) { continue;}
                    }
                    else if (x == 1 && y == -1)//SE
                    {
                        if (!tiles[current.x, current.y - 1].passable
                        && !tiles[current.x + 1, current.y].passable) { continue; }
                    }
                    else if (x == -1 && y == -1)//SW
                    {
                        if (!tiles[current.x, current.y - 1].passable
                        && !tiles[current.x - 1, current.y].passable) { continue; }
                    }
                    else if (x == -1 && y == 1)//NW
                    {
                        if (!tiles[current.x, current.y + 1].passable
                        && !tiles[current.x - 1, current.y].passable) { continue; }
                    }
                }

                int newMoveCostToNeighbor = current.gCost + GetDistanceBetweenNodes(current, neighbor);
                if (newMoveCostToNeighbor < neighbor.gCost || !open.Contains(neighbor))
                {
                    neighbor.gCost = newMoveCostToNeighbor;
                    neighbor.hCost = GetDistanceBetweenNodes(neighbor, target);
                    neighbor.parent = current;
                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }

        }
        return null;
    }

    int GetDistanceBetweenNodes(tileCord A, tileCord B)
    {
        int distX = Mathf.Abs(A.x - B.x);
        int distY = Mathf.Abs(A.y - B.y);
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }

    List<Tileboard.direction> Retrace(tileCord start, tileCord end)
    {
        List<Tileboard.direction> path = new List<Tileboard.direction>();
        tileCord currentTile = end;

        while (currentTile != start)
        {
            int xVec = currentTile.x - currentTile.parent.x;
            int yVec = currentTile.y - currentTile.parent.y;

            if (xVec == 0 && yVec == 1)
            {
                path.Add(Tileboard.direction.N);
            }
            else if (xVec == 1 && yVec == 1)
            {
                path.Add(Tileboard.direction.NE);
            }
            else if (xVec == 1 && yVec == 0)
            {
                path.Add(Tileboard.direction.E);
            }
            else if (xVec == 1 && yVec == -1)
            {
                path.Add(Tileboard.direction.SE);
            }
            else if (xVec == 0 && yVec == -1)
            {
                path.Add(Tileboard.direction.S);
            }
            else if (xVec == -1 && yVec == -1)
            {
                path.Add(Tileboard.direction.SW);
            }
            else if (xVec == -1 && yVec == 0)
            {
                path.Add(Tileboard.direction.W);
            }
            else if (xVec == -1 && yVec == 1)
            {
                path.Add(Tileboard.direction.NW);
            }

            currentTile = currentTile.parent;
        }
        path.Reverse();
        return path;
    }

}
