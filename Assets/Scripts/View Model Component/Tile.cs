using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
To do
more doc strings
 */

/// <summary>
/// In Combat and WA scene, the game board is represented by a board object which
/// holds a list of Tile object. The Tile objects have some GameObject properties
/// for visual displays and game related properties like units that are on the tile.
/// Tiles are created, called and manipulated from Board.cs
/// </summary>
public class Tile : MonoBehaviour
{
	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Material material;

	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Texture team2Highlight;

	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Texture team3Highlight;

	/// <summary>
	///
	/// </summary>
	[SerializeField]
	Texture tileHighlight;

	/// <summary>
	/// In WA mode can exit map and go to a different map by stepping on certain tiles.
	/// Material for indicating a tile is able to exit the map.
	/// </summary>
	[SerializeField]
	Material tileExitMapMaterial;

	#region Const
	/// <summary>
	/// Used for scaling the height of a tile
	/// </summary>
	public const float stepHeight = 0.25f;

	/// <summary>
	/// used for centering the indicator for the tile
	/// used for centering the height of units that walk around the map
	/// </summary>
	public const float centerHeight = 0.45f;
	#endregion

	#region Fields / Properties
	/// <summary>
	/// Contains the x and y coordinates of a tile
	/// </summary>
	public Point pos;

	/// <summary>
	/// Has the z coordinates (height) of the tile
	/// </summary>
	public int height;

	public Vector3 center { get { return new Vector3(pos.x, height * stepHeight + centerHeight, pos.y); } }
	public GameObject content;

	/// <summary>
	/// When calculating valid movements in cref, use the prev field
	/// </summary>
	[HideInInspector] public Tile prev;

	/// <summary>
	/// WHen calculating valid movements in cref, use the distance to track how many
	/// additional tiles the unit can move to
	/// </summary>
	[HideInInspector] public int distance;

	/// <summary>
	/// walkAroundMode: multiple units can be moving around at the 
	/// same time so need dictionaries to distinguish unique prev and unique distance 
	/// </summary>
	public Dictionary<int, Tile> prevDict = new Dictionary<int, Tile>();

	/// <summary>
	/// walkAroundMode: multiple units can be moving around at the 
	/// same time so need dictionaries to distinguish unique prev and unique distance 
	/// </summary>
	public Dictionary<int, int> distanceDict = new Dictionary<int, int>();

	private int unitId = NameAll.NULL_UNIT_ID;

	/// <summary>
	/// added to easier check tiles for PlayerUnits
	/// </summary>
	public int UnitId
	{
		get { return unitId; }
		set { unitId = value; }
	}

	private int pickUpId = 0; //0 for nothing, 1 for crystals

	/// <summary>
	/// Can sometimes be objects on tiles
	/// for now, 0 for nothing, 1 for crystals
	/// to do: add enums for this type
	/// </summary>
	public int PickUpId
	{
		get { return pickUpId; }
		set { pickUpId = value; }
	}

	private int tileType = NameAll.TILE_TYPE_DEFAULT;
	/// <summary>
	/// for WA mode Tile Type can be exit map as well
	/// </summary>
	public int TileType
	{
		get { return tileType; }
		set { tileType = value; }
	}
	#endregion

	#region Public

	/// <summary>
	/// increase the height of a tile
	/// </summary>
	public void Grow()
	{
		height++;
		Match();
	}

	/// <summary>
	/// reduce the height of a tile
	/// </summary>
	public void Shrink()
	{
		height--;
		Match();
	}

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Point p, int h)
	{
		pos = p;
		height = h;
		Match();
	}

	/// <summary>
	///
	/// </summary>
	public void Load(Vector3 v)
	{
		Load(new Point((int)v.x, (int)v.z), (int)v.y);
		this.UnitId = NameAll.NULL_UNIT_ID;
	}

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Vector3 v, int renderMode)
	{
		Load(new Point((int)v.x, (int)v.z), (int)v.y, renderMode);
		this.UnitId = NameAll.NULL_UNIT_ID;
	}

	/// <summary>
	/// Load the Tile. I think tiles are loaded when created by the board
	/// </summary>
	public void Load(Point p, int h, int renderMode)
	{
		pos = p;
		height = h;
		if (renderMode != NameAll.PP_RENDER_NONE)
			Match();

	}

	/// <summary>
	/// change tile look for MapBuilder Scene
	/// </summary>
	public void RevertTile()
	{
		var rend = this.gameObject.GetComponent<Renderer>();
		if (this.TileType == NameAll.TILE_TYPE_EXIT_MAP)
		{
			rend.material = tileExitMapMaterial;
		}
		else
		{
			rend.material = material;
		}
	}

	/// <summary>
	/// Change texture of tile to highlight it for a specific team or look
	/// </summary>
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
	/// <summary>
	/// When a tile is loaded or height changed, call this to line the tile up correctly
	/// </summary>
	void Match()
	{
		transform.localPosition = new Vector3(pos.x, height * stepHeight / 2f, pos.y);
		transform.localScale = new Vector3(1, height * stepHeight, 1);
	}

	//private IEnumerator AutoRevert()
	//{
	//    yield return new WaitForSeconds(3);
	//    RevertTile();
	//}
	#endregion

	/// <summary>
	/// Get info on tile for debugging or display purposes in UI Target panel
	/// </summary>
	public string GetTileSummary()
	{
		return " (" + this.pos.x + "," + this.pos.y + ")" + " unitID: " + this.unitId + " height: " + this.height;
	}
}
