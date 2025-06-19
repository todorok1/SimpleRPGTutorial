using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘機能を呼び出すイベントを処理するクラスです。
    /// </summary>
    public class EventProcessBattle : EventProcessBase, IPostBattle
    {
        /// <summary>
        /// 戦闘の対象の敵キャラクターIDです。
        /// </summary>
        [SerializeField]
        int _enemyId;

        /// <summary>
        /// 戦闘に負けた時に呼ばれるイベントの処理です。
        /// </summary>
        [SerializeField]
        EventProcessBase _loseProcess;

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public override void Execute()
        {
            var battleManager = FindAnyObjectByType<BattleManager>();
            if (battleManager == null)
            {
                SimpleLogger.Instance.LogError("シーン内のBattleManagerが見つかりませんでした。");
                CallNextProcess();
            }
            
            battleManager.SetUpEnemyStatus(_enemyId);
            battleManager.RegisterCallback(this);
            battleManager.StartBattle();
        }

        /// <summary>
        /// 戦闘終了時のコールバックです。
        /// </summary>
        public void OnFinishedBattle()
        {
            CallNextProcess();
        }

        /// <summary>
        /// 戦闘で負けた時のコールバックです。
        /// </summary>
        public void OnLostBattle()
        {
            CallNextProcess(_loseProcess);
        }
    }
}