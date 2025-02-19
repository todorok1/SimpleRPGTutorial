using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘中のアクションを登録するクラスです。
    /// </summary>
    public class BattleActionRegister : MonoBehaviour
    {
        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        BattleActionProcessor _actionProcessor;

        /// <summary>
        /// このクラスを初期化します。
        /// </summary>
        public void InitializeRegister(BattleActionProcessor actionProcessor)
        {
            _actionProcessor = actionProcessor;
        }

        /// <summary>
        /// キャラクターのパラメータレコードを取得します。
        /// </summary>
        public ParameterRecord GetCharacterParameterRecord(int characterId)
        {
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            var parameterTable = CharacterDataManager.GetParameterTable(characterId);
            var parameterRecord = parameterTable.parameterRecords.Find(p => p.level == characterStatus.level);
            return parameterRecord;
        }

        /// <summary>
        /// 攻撃コマンドのアクションをセットします。
        /// </summary>
        public void SetFriendAttackAction(int actorId, int targetId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                targetId = targetId,
                isTargetFriend = false,
                battleCommand = BattleCommand.Attack,
                actorSpeed = characterParam.speed,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 敵キャラクターの攻撃コマンドのアクションをセットします。
        /// </summary>
        public void SetEnemyAttackAction(int actorId, int targetId, EnemyData enemyData)
        {
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = false,
                targetId = targetId,
                isTargetFriend = true,
                battleCommand = BattleCommand.Attack,
                actorSpeed = enemyData.speed,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 魔法コマンドのアクションをセットします。
        /// </summary>
        public void SetFriendMagicAction(int actorId, int targetId, int magicId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                targetId = targetId,
                battleCommand = BattleCommand.Magic,
                itemId = magicId,
                actorSpeed = characterParam.speed,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 敵キャラクターの魔法コマンドのアクションをセットします。
        /// </summary>
        public void SetEnemyMagicAction(int actorId, int targetId, int magicId, EnemyData enemyData)
        {
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = false,
                targetId = targetId,
                battleCommand = BattleCommand.Magic,
                itemId = magicId,
                actorSpeed = enemyData.speed,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// アイテムコマンドのアクションをセットします。
        /// </summary>
        public void SetFriendItemAction(int actorId, int enemyBattleId, int itemId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);

            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogError($"選択されたIDのアイテムは見つかりませんでした。ID : {itemId}");
                return;
            }

            int targetId = enemyBattleId;
            bool isTargetFriend = false;
            if (itemData.itemEffect.effectTarget == EffectTarget.Own
                || itemData.itemEffect.effectTarget == EffectTarget.FriendAll
                || itemData.itemEffect.effectTarget == EffectTarget.FriendSolo)
            {
                isTargetFriend = true;
                targetId = actorId;
            }

            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                targetId = targetId,
                isTargetFriend = isTargetFriend,
                battleCommand = BattleCommand.Item,
                itemId = itemId,
                actorSpeed = characterParam.speed,
            };

            _actionProcessor.RegisterAction(action);
        }

        /// <summary>
        /// 逃げるコマンドのアクションをセットします。
        /// </summary>
        public void SetFriendRunAction(int actorId)
        {
            var characterParam = GetCharacterParameterRecord(actorId);
            BattleAction action = new()
            {
                actorId = actorId,
                isActorFriend = true,
                battleCommand = BattleCommand.Run,
                actorSpeed = characterParam.speed,
            };

            _actionProcessor.RegisterAction(action);
        }
    }
}