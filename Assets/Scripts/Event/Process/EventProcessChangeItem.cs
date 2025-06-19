using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// アイテムを増減させるイベントを処理するクラスです。
    /// </summary>
    public class EventProcessChangeItem : EventProcessBase
    {
        /// <summary>
        /// 変化させる対象のアイテムIDです。
        /// </summary>
        [SerializeField]
        int _itemId;

        /// <summary>
        /// 増減させる量です。
        /// </summary>
        [SerializeField]
        int _itemNum;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            CharacterStatusManager.IncreaseItem(_itemId, _itemNum);
            CallNextProcess();
        }
    }
}