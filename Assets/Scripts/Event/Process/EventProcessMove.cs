using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 場所移動に関するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessMove : EventProcessBase
    {
        /// <summary>
        /// 移動先のマップIDです。
        /// </summary>
        [SerializeField]
        int _targetMapId;

        /// <summary>
        /// 移動先の座標です。
        /// </summary>
        [SerializeField]
        Vector3Int _targetPosition;

        /// <summary>
        /// マップ機能を管理するクラスへの参照です。
        /// </summary>
        MapManager _mapManager;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            SetUpReference();
            if (_mapManager == null)
            {
                SimpleLogger.Instance.LogError("MapManagerが見つかりません。");
                CallNextProcess();
                return;
            }

            _mapManager.ShowMap(_targetMapId);

            // 主人公のゲームオブジェクトを取得します。
            var playerMover = FindAnyObjectByType<PlayerMover>();
            if (playerMover == null)
            {
                SimpleLogger.Instance.LogError("PlayerMoverが見つかりません。");
                CallNextProcess();
                return;
            }
            playerMover.SetPosition(_targetPosition);

            CallNextProcess();
        }

        /// <summary>
        /// 必要な参照をセットします。
        /// </summary>
        void SetUpReference()
        {
            _mapManager = FindAnyObjectByType<MapManager>();
        }
    }
}