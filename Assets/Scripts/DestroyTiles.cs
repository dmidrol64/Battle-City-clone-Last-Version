using UnityEngine.Tilemaps;
using UnityEngine;

public class DestroyTiles : MonoBehaviour,IDamageable
{

    [SerializeField] private Tilemap tilemap;
    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        Vector3Int cellPos=  tilemap.WorldToCell(hitPosition);

        if (!tilemap.cellBounds.Contains(cellPos)) return;

        TileBase currentTile = tilemap.GetTile(cellPos);

        if (currentTile != null) 
        {
            tilemap.SetTile(cellPos, null);
            tilemap.RefreshTile(cellPos);
        }
    }
}
