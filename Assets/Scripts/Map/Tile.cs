using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    // Q + R + S = 0;
    public readonly int Q;  // stupac
    public readonly int R;  // red
    public readonly int S;  // suma

    public GameObject tileGO { get; set; }

    public Unit unit { get; set; }

    public bool hasForest = false;
    public float elevation = 0f;
    public bool hasPortal = false;

    //public List<Item> groundItems = new List<Item>();
    public int itemID = 0;

    public readonly TileMap tileMap;

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    public Tile(TileMap tileMap, int q, int r)
    {
        this.tileMap = tileMap;

        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }

    //public Tile(Tile tile)
    //{
    //    this.tileMap = tile.tileMap;
    //    this.Q = tile.Q;
    //    this.R = tile.R;
    //    this.S = tile.S;
    //}

    //public bool HasForest
    //{
    //    get { return hasForest; }
    //    set { hasForest = value; }
    //}
    //public float elevation
    //{
    //    get { return elevation; }
    //    set { elevation = value; }
    //}

    public Vector3 Position()
    {
        float radius = 1f;
        float height = radius * 2;
        float width = WIDTH_MULTIPLIER * height;

        float vertical_spacing = height * 0.75f;
        float horizontal_spacing = width;

        return new Vector3(
            horizontal_spacing * (this.Q + this.R / 2f),
            0,
            vertical_spacing * this.R
            );
    }

    //public string ReturnStats()
    //{
    //    string outputString = "";

    //    if (elevation >= 0.7f) outputString += "Mountain\n";
    //    else if (elevation < 0.7f && elevation >= 0.3f) outputString += "Hill\n";
    //    else if (elevation > -0.7f && elevation <= -0.3f) outputString += "Swamp\n";
    //    else outputString += "Plains\n";

    //    return outputString;
    //}

    //public string ReturnGroundItems()
    //{
    //    string output = "";
    //    if (groundItems == null)
    //    {
    //        output = "No items on ground.";
    //    }
    //    else
    //    {
    //        foreach (Item item in groundItems)
    //        {
    //            output += item.name + ",   ";
    //        }
    //    }
        
    //    return output;
    //}

    //public Vector2Int ReturnAxial()
    //{
    //    return new Vector2Int(Q, R);
    //}

    //public Vector3Int ReturnCube()
    //{
    //    return new Vector3Int(Q, R, S);
    //}

    //public void CreateItemOnTile(Item item)
    //{

    //}
    //public Item CreateRandomItemOnTile()
    //{
    //    Item item = ItemDB.GetItem(Random.Range(0, 21));
    //    groundItems.Add(item);
    //    return item;
    //}
}
