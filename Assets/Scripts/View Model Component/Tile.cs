using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
    [SerializeField]
    Material material;
    [SerializeField]
    Texture team2Highlight;
    [SerializeField]
    Texture team3Highlight;
    [SerializeField]
    Texture tileHighlight;
	[SerializeField]
	Material tileExitMapMaterial;

	#region Const
	public const float stepHeight = 0.25f;
	//used for centering the indicator for the tile
	//used for centering the height of units that walk around the map
	public const float centerHeight = 0.45f;
	#endregion

	#region Fields / Properties
	public Point pos;
	public int height;

	public Vector3 center { get { return new Vector3(pos.x, height * stepHeight + centerHeight, pos.y); }}
	public GameObject content;
	[HideInInspector] public Tile prev;
	[HideInInspector] public int distance;

    //walkAroundMode: multiple units can be moving around at the same time so need dictionaries to distinguish unique prev and unique distance
    public Dictionary<int, Tile> prevDict = new Dictionary<int, Tile>();
    public Dictionary<int, int> distanceDict = new Dictionary<int, int>();

    private int unitId = NameAll.NULL_UNIT_ID; //added to easier check tiles for PlayerUnits
    public int UnitId
    {
        get { return unitId; }
        set { unitId = value; }
    }

    private int pickUpId = 0; //0 for nothing, 1 for crystals
    public int PickUpId
    {
        get { return pickUpId;}
        set { pickUpId = value; }
    }

	private int tileType = NameAll.TILE_TYPE_DEFAULT; //for WA mode Tile Type can be exit map as well
	public int TileType
	{
		get { return tileType; }
		set { tileType = value; }
	}
	#endregion

	#region Public
	public void Grow ()
	{
		height++;
		Match();
	}
	
	public void Shrink ()
	{
		height--;
		Match ();
	}

	public void Load (Point p, int h)
	{
		pos = p;
		height = h;
		Match();
	}

	public void Load (Vector3 v)
	{
		Load (new Point((int)v.x, (int)v.z), (int)v.y);
        this.UnitId = NameAll.NULL_UNIT_ID;
	}

	public void Load(Vector3 v, int renderMode)
	{
		Load(new Point((int)v.x, (int)v.z), (int)v.y, renderMode);
		this.UnitId = NameAll.NULL_UNIT_ID;
	}

	public void Load(Point p, int h, int renderMode)
	{
		pos = p;
		height = h;
		if(renderMode != NameAll.PP_RENDER_NONE)
			Match();

	}

	//change tile look for MapBuilder
	public void RevertTile()
    {
        var rend = this.gameObject.GetComponent<Renderer>();
		if( this.TileType == NameAll.TILE_TYPE_EXIT_MAP)
		{
			rend.material = tileExitMapMaterial;
		}
		else
		{
			rend.material = material;
		} 
    }

    public void HighlightTile(int teamId)
    {
        if (teamId == 2)
        {
            var rend = this.gameObject.GetComponent<Renderer>();
            //rend.material.mainTexture = Resources.Load("square frame 1") as Texture;
            rend.material.mainTexture = team2Highlight;
        }
        else if (teamId == 3)
        {
            var rend = this.gameObject.GetComponent<Renderer>();
            //rend.material.mainTexture = Resources.Load("square frame 2") as Texture;
            rend.material.mainTexture = team3Highlight;
        }
        else
        {
            var rend = this.gameObject.GetComponent<Renderer>();
            //rend.material.mainTexture = Resources.Load("square frame 3") as Texture;
            rend.material.mainTexture = tileHighlight;
        }
    }
    #endregion

    #region Private
    void Match ()
	{
		transform.localPosition = new Vector3( pos.x, height * stepHeight / 2f, pos.y );
		transform.localScale = new Vector3(1, height * stepHeight, 1);
	}

    //private IEnumerator AutoRevert()
    //{
    //    yield return new WaitForSeconds(3);
    //    RevertTile();
    //}
    #endregion

    public string GetTileSummary()
    {
        return " (" + this.pos.x + "," + this.pos.y + ")" + " unitID: " + this.unitId + " height: " + this.height;
    }
}
