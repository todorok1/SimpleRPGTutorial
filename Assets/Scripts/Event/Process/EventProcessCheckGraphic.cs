using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントオブジェクトの画像を確認するイベントを処理するクラスです。
    /// </summary>
    public class EventProcessCheckGraphic : EventProcessBase
    {
        /// <summary>
        /// グラフィックの確認範囲です。
        /// </summary>
        [SerializeField]
        CheckRange _checkRange = CheckRange.Self;

        enum CheckRange
        {
            Self,
            All
        }

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            if (_checkRange == CheckRange.Self)
            {
                CheckGraphicSelf();
            }
            else if (_checkRange == CheckRange.All)
            {
                CheckGraphicAll();
            }
        }

        /// <summary>
        /// このイベントオブジェクトの画像を確認します。
        /// </summary>
        void CheckGraphicSelf()
        {
            SimpleLogger.Instance.Log($"CheckGraphicSelf()が呼ばれました。_eventFileData != null : {_eventFileData != null}");
            if (_eventFileData != null)
            {
                _eventFileData.SetEventGraphic();
            }
            CallNextProcess();
        }

        /// <summary>
        /// マップ内のイベントオブジェクトの画像を確認します。
        /// </summary>
        void CheckGraphicAll()
        {
            StartCoroutine(CheckGraphicAllProcess());
        }

        /// <summary>
        /// マップ内のイベントオブジェクトの画像を確認します。
        /// </summary>
        IEnumerator CheckGraphicAllProcess()
        {
            var mapManager = FindAnyObjectByType<MapManager>();
            if (mapManager != null)
            {
                var eventFiles = mapManager.GetEventsInMap();
                yield return StartCoroutine(mapManager.CheckEventGraphic(eventFiles));
                CallNextProcess();
            }
            else
            {
                SimpleLogger.Instance.LogWarning("シーン内にMapManagerが見つかりませんでした。");
                CallNextProcess();
            }
        }

        /// <summary>
        /// フェードが完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedFade()
        {
            CallNextProcess();
        }
    }
}