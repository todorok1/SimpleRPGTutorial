using UnityEngine;
using TMPro;

namespace SimpleRpg
{
    /// <summary>
    /// ステータス表示のUIを制御するクラスです。
    /// </summary>
    public class BattleUIControllerStatus : BattleUIControllerBase
    {
        /// <summary>
        /// キャラクターの名前を表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _characterNameText;

        /// <summary>
        /// 現在のHPを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _currentHpText;

        /// <summary>
        /// 最大HPを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _maxHpText;

        /// <summary>
        /// 現在のMPを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _currentMpText;

        /// <summary>
        /// 最大MPを表示するテキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _maxMpText;

        /// <summary>
        /// キャラクターの名前をセットします。
        /// </summary>
        /// <param name="characterName">キャラクターの名前</param>
        public void SetCharacterName(string characterName)
        {
            _characterNameText.text = characterName;
        }

        /// <summary>
        /// 現在のHPをセットします。
        /// </summary>
        /// <param name="currentHp">現在のHP</param>
        public void SetCurrentHp(int currentHp)
        {
            _currentHpText.text = currentHp.ToString();
        }

        /// <summary>
        /// 最大HPをセットします。
        /// </summary>
        /// <param name="maxHp">最大HP</param>
        public void SetMaxHp(int maxHp)
        {
            _maxHpText.text = maxHp.ToString();
        }

        /// <summary>
        /// 現在のMPをセットします。
        /// </summary>
        /// <param name="currentMp">現在のMP</param>
        public void SetCurrentMp(int currentMp)
        {
            _currentMpText.text = currentMp.ToString();
        }

        /// <summary>
        /// 最大MPをセットします。
        /// </summary>
        /// <param name="maxMp">最大MP</param>
        public void SetMaxMp(int maxMp)
        {
            _maxMpText.text = maxMp.ToString();
        }

        /// <summary>
        /// キャラクターのステータスを全てセットします。
        /// </summary>
        /// <param name="characterStatus">キャラクターのステータス</param>
        public void SetCharacterStatus(CharacterStatus characterStatus)
        {
            if (characterStatus == null)
            {
                SimpleLogger.Instance.LogWarning("キャラクターステータスがnullです。");
                return;
            }

            var characterName = CharacterDataManager.GetCharacterName(characterStatus.characterId);
            SetCharacterName(characterName);

            var level = characterStatus.level;
            var parameterTable = CharacterDataManager.GetParameterTable(characterStatus.characterId);
            var record = parameterTable.parameterRecords.Find(r => r.level == level);

            SetCurrentHp(characterStatus.currentHp);
            SetMaxHp(record.hp);
            SetCurrentMp(characterStatus.currentMp);
            SetMaxMp(record.mp);
        }

        /// <summary>
        /// 全キャラクターのステータスを更新します。
        /// </summary>
        public void UpdateAllCharacterStatus()
        {
            foreach (var characterId in CharacterStatusManager.partyCharacter)
            {
                var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
                SetCharacterStatus(characterStatus);
            }
        }
    }
}