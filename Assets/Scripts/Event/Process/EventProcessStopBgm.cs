using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// BGMを停止させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessStopBgm : EventProcessBase
    {
        /// <summary>
        /// フェードにかかる時間です。
        /// </summary>
        [SerializeField]
        float _fadeTime = 0.5f;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            AudioManager.Instance.StopAllBgm(_fadeTime);
            CallNextProcess();
        }
    }
}