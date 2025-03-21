using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中の魔法アクションを処理するクラスです。
    /// </summary>
    public class BattleActionProcessorMagic : MonoBehaviour, IBattleActionProcessor
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
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
        /// </summary>
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 魔法効果をポーズするかどうかのフラグです。
        /// </summary>
        bool _pauseMagicEffect;

        /// <summary>
        /// 参照をセットします。
        /// </summary>
        public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
        {
            _battleManager = battleManager;
            _actionProcessor = actionProcessor;
            _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
            _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        }

        /// <summary>
        /// 魔法のアクションを処理します。
        /// </summary>
        public void ProcessAction(BattleAction action)
        {
            var magicData = MagicDataManager.GetMagicDataById(action.itemId);

            // 消費MPの分だけMPを減らします。
            int hpDelta = 0;
            int mpDelta = magicData.cost * -1;
            if (action.isActorFriend)
            {
                CharacterStatusManager.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
            }
            else
            {
                _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ProcessMagicActionCoroutine(action, magicData));
        }

        /// <summary>
        /// 魔法のアクションを処理するコルーチンです。
        /// </summary>
        IEnumerator ProcessMagicActionCoroutine(BattleAction action, MagicData magicData)
        {
            // 魔法の効果を処理します。
            foreach (var magicEffect in magicData.magicEffects)
            {
                if (magicEffect.magicCategory == MagicCategory.Recovery)
                {
                    int hpDelta = BattleCalculator.CalculateHealValue(magicEffect.value);
                    int mpDelta = 0;
                    bool isMagicTargetFriend = IsMagicTargetFriend(magicEffect);
                    if (action.isActorFriend && isMagicTargetFriend)
                    {
                        if (action.isActorFriend && isMagicTargetFriend)
                        {
                            CharacterStatusManager.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
                            action.targetId = action.actorId;
                            action.isTargetFriend = true;
                        }
                        else
                        {
                            _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);
                            action.targetId = action.actorId;
                            action.isTargetFriend = false;
                        }
                    }
                    else
                    {
                        if (action.isActorFriend && isMagicTargetFriend)
                        {
                            CharacterStatusManager.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
                            action.targetId = action.actorId;
                            action.isTargetFriend = true;
                        }
                        else
                        {
                            _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);
                            action.targetId = action.actorId;
                            action.isTargetFriend = false;
                        }
                    }

                    _pauseMagicEffect = true;
                    StartCoroutine(ShowMagicHealMessage(action, magicData.magicName, hpDelta));
                }
                else
                {
                    Debug.LogWarning($"未定義の魔法効果です。 ID: {magicData.magicId}");
                }

                while (_pauseMagicEffect)
                {
                    yield return null;
                }
            }

            _actionProcessor.SetPauseProcess(false);
        }

        /// <summary>
        /// 回復魔法のメッセージを表示します。
        /// </summary>
        IEnumerator ShowMagicHealMessage(BattleAction action, string magicName, int healValue)
        {
            string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
            string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateMagicCastMessage(actorName, magicName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateHpHealMessage(targetName, healValue);
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            _pauseMagicEffect = false;
        }

        /// <summary>
        /// 魔法の対象が味方かどうかを判定します。
        /// </summary>
        bool IsMagicTargetFriend(MagicEffect magicEffect)
        {
            bool isFriend = false;
            if (magicEffect.effectTarget == EffectTarget.Own
                || magicEffect.effectTarget == EffectTarget.FriendSolo
                || magicEffect.effectTarget == EffectTarget.FriendAll)
            {
                isFriend = true;
            }
            return isFriend;
        }
    }
}