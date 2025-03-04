using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の逃走アクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorRun : MonoBehaviour
    {
        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        BattleActionProcessor _actionProcessor;

        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// メッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MessageWindowController _messageWindowController;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        }

        /// <summary>
        /// 逃走のアクションを処理します。
        /// </summary>
        public void ProcessRunAction(BattleAction action)
        {
            var actorStatus = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
            var targetStatus = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);

            _actionProcessor.SetPauseProcess(true);

            // 逃走が成功したかどうかを判定します。
            bool isRunSuccess = BattleCalculator.CalculateCanRun(actorStatus.speed, targetStatus.speed);
            StartCoroutine(ShowRunMessage(action, isRunSuccess));
        }

        /// <summary>
        /// 逃走のメッセージを表示します。
        /// </summary>
        IEnumerator ShowRunMessage(BattleAction action, bool isSuccess)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateRunMessage(actorName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            SimpleLogger.Instance.Log($"キャラクターの逃走判定 : {isSuccess}");

            if (isSuccess)
            {
                if (action.isActorFriend)
                {
                    _battleManager.OnRunaway();
                }
                else
                {
                    _battleManager.OnEnemyRunaway();
                }
            }
            else
            {
                _actionProcessor.SetPauseMessage(true);
                _messageWindowController.GenerateRunFailedMessage();
                while (_actionProcessor.IsPausedMessage)
                {
                    yield return null;
                }
                _actionProcessor.SetPauseProcess(false);
            }
        }
    }
}