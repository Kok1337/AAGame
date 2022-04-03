using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class BackgroundManager : MonoBehaviour
{
	private Tilemap _tilemap;

	[SerializeField]
	private List<DataTile> _tileDatas;
	[SerializeField]
	private byte _defaultPenalty;

	private Dictionary<TileBase, DataTile> _dataFromTiles;

	private void Awake()
	{
		_tilemap = GetComponent<Tilemap>();
		_dataFromTiles = new Dictionary<TileBase, DataTile>();
		foreach (var tileData in _tileDatas)
		{
			foreach (var tile in tileData.tiles)
			{
				_dataFromTiles.Add(tile, tileData);
			}
		}
	}

	void Start()
    {
		
	}

	public byte GetPenalty(int x, int y)
	{
		Vector3Int tilePosition = new Vector3Int(x, y, 0);
		TileBase tile = _tilemap.GetTile(tilePosition);

		if (tile == null || _dataFromTiles.ContainsKey(tile) == false)
		{
			return _defaultPenalty;
		}

		byte penalty = _dataFromTiles[tile].penalty;
		return penalty;
	}

	public float GetWalkingSpeed(Vector3 worldPosition)
	{
		Vector3Int tilePosition = _tilemap.WorldToCell(worldPosition);
		TileBase tile = _tilemap.GetTile(tilePosition);

		if (tile == null || _dataFromTiles.ContainsKey(tile) == false)
		{
			return 1;
		}

		float walkingSpeed = _dataFromTiles[tile].walkingSpeed;
		return walkingSpeed;
	}
}
