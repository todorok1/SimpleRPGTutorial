using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘機能を呼び出すイベントを処理するクラスです。
    /// </summary>
    public class EventProcessBattleLose : EventProcessBase, IFadeCallback
    {
        /// <summary>
        /// フェードインしている状態か、フェードアウトしている状態かを示すフラグです。
        /// </summary>
        bool _isFadeOut = false;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            // 戦闘に負けたフラグをセットします。
            FlagManager.Instance.SetFlagState(FlagNames.BattleLose, true);

            // 画面をフェードアウトさせます。
            _isFadeOut = true;
            FadeManager.Instance.SetCallback(this);
            FadeManager.Instance.FadeOutScreen(0.5f);
        }

        /// <summary>
        /// マップの移動処理を行います。
        /// </summary>
        void MoveMap()
        {
            // マップを移動します。
            int firstMapId = 1;
            var mapManager = FindAnyObjectByType<MapManager>();
            if (mapManager != null)
            {
                mapManager.ShowMap(firstMapId);
            }
            else
            {
                SimpleLogger.Instance.LogError("シーン内にMapManagerが見つかりませんでした。");
            }

            // 画面をフェードインさせます。
            _isFadeOut = false;
            FadeManager.Instance.FadeInScreen(0.5f);
        }

        /// <summary>
        /// フェードが完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedFade()
        {
            if (_isFadeOut)
            {
                MoveMap();
            }
            else
            {
                CallNextProcess();
            }
        }
    }
}