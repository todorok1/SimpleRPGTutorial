using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SimpleRpg
{
    /// <summary>
    /// Tilemapに関する機能を提供する管理クラスです。
    /// </summary>
    public class TilemapManager : MonoBehaviour
    {
        /// <summary>
        /// 地面用Tilemapへの参照です。
        /// </summary>
        [SerializeField]
        Tilemap _tilemapBase;

        /// <summary>
        /// 装飾品用Tilemapへの参照です。
        /// </summary>
        [SerializeField]
        Tilemap _tilemapProps;

        /// <summary>
        /// オーバーレイ用Tilemapへの参照です。
        /// </summary>
        [SerializeField]
        Tilemap _tilemapOverray;

        /// <summary>
        /// 侵入できないタイル一覧の定義ファイルです。
        /// </summary>
        [SerializeField]
        NoEntryTileData _noEntryTileData;

        /// <summary>
        /// ワールド座標からTilemap上の論理的な座標を取得します。
        /// </summary>
        /// <param name="worldPos">変換元のワールド座標</param>
        public Vector3Int GetPositionOnTilemap(Vector3 worldPos)
        {
            var posOnTile = Vector3Int.zero;
            if (_tilemapBase != null)
            {
                posOnTile =_tilemapBase.WorldToCell(worldPos);
            }
            return posOnTile;
        }

        /// <summary>
        /// Tilemap上の座標からワールド座標を取得します。
        /// </summary>
        /// <param name="worldPos">変換元のTilemap上の座標</param>
        public Vector3 GetWorldPosition(Vector3Int posOnTile)
        {
            var worldPos = Vector3.zero;
            if (_tilemapBase != null)
            {
                worldPos = _tilemapBase.CellToWorld(posOnTile);
                worldPos += _tilemapBase.tileAnchor;
            }
            return worldPos;
        }

        /// <summary>
        /// 対象の座標に移動できるかどうかを確認します。
        /// </summary>
        /// <param name="targetPos">移動先のTilemap上の座標</param>
        public bool CanEntryTile(Vector3Int targetPos)
        {
            bool canEntry = true;

            if (_noEntryTileData == null)
            {
                Debug.LogWarning("侵入できないタイル一覧の定義ファイルがnullです。[NoEntryTileData]のフィールドに定義ファイルをアサインしてください。");
                return canEntry;
            }

            // 各レイヤーのタイルを確認します。
            List<TileBase> targetTiles = new();
            var baseTile = GetTileOnPos(_tilemapBase, targetPos);
            targetTiles.Add(baseTile);

            var propsTile = GetTileOnPos(_tilemapProps, targetPos);
            targetTiles.Add(propsTile);

            var overrayTile = GetTileOnPos(_tilemapOverray, targetPos);
            targetTiles.Add(overrayTile);

            // 定義ファイルのタイルと照合します。
            foreach (var tile in targetTiles)
            {
                if (_noEntryTileData.noEntryTiles == null)
                {
                    break;
                }

                if (_noEntryTileData.noEntryTiles.Contains(tile))
                {
                    canEntry = false;
                    break;
                }
            }

            return canEntry;
        }

        /// <summary>
        /// 対象のTilemapにて、指定した位置のタイルを取得します。
        /// </summary>
        /// <param name="tilemap">対象のTilemap</param>
        /// <param name="targetPos">Tilemap上の座標</param>
        TileBase GetTileOnPos(Tilemap tilemap, Vector3Int targetPos)
        {
            TileBase tile = null;
            if (tilemap != null)
            {
                tile = tilemap.GetTile(targetPos);
            }
            return tile;
        }
    }
}