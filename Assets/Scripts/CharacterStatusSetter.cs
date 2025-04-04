using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 味方キャラクターのステータスをセットアップするクラスです。
    /// </summary>
    public class CharacterStatusSetter : MonoBehaviour
    {
        /// <summary>
        /// 味方キャラクターのステータスをセットします。
        /// </summary>
        [SerializeField]
        List<int> _partyCharacters = new() {1};

        void Update()
        {
            if (Time.frameCount == 5)
            {
                SetPlayerStatus();
            }
        }

        /// <summary>
        /// 味方キャラクターのステータスをセットします。
        /// </summary>
        void SetPlayerStatus()
        {
            var exp = 0;
            var level = 1;
            List<CharacterStatus> characterStatuses = new();

            foreach (int characterId in _partyCharacters)
            {
                // レベルに対応するパラメータデータを取得します。
                var parameterTable = CharacterDataManager.GetParameterTable(characterId);
                var parameterRecord = parameterTable.parameterRecords.Find(record => record.level == level);

                // 指定したレベルまでに覚えている魔法のIDをリスト化します。
                var magicList = GetMagicIdList(characterId, level);

                // キャラクターのステータスを設定します。
                CharacterStatus status = new()
                {
                    characterId = characterId,
                    level = level,
                    exp = exp,
                    currentHp = parameterRecord.hp,
                    currentMp = parameterRecord.mp,
                    equipWeaponId = 0,
                    equipArmorId = 0,
                    magicList = magicList,
                };

                characterStatuses.Add(status);
            }

            CharacterStatusManager.characterStatuses = characterStatuses;

            // パーティにいるキャラクターのIDをセットします。
            CharacterStatusManager.partyCharacter = _partyCharacters;

            // 所持アイテムをセットします。
            CharacterStatusManager.partyItemInfoList = new();

            // 所持金をセットします。
            CharacterStatusManager.partyGold = 0;
        }

        /// <summary>
        /// キャラクターが覚えている魔法のIDリストを返します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <param name="level">キャラクターのレベル</param>
        List<int> GetMagicIdList(int characterId, int level)
        {
            var learnableMagic = CharacterDataManager.GetLearnableMagic(characterId, level);
            List<int> magicList = new();
            foreach (var record in learnableMagic)
            {
                magicList.Add(record.magicId);
            }
            return magicList;
        }
    }
}