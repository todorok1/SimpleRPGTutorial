using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターのステータスを管理するクラスです。
    /// </summary>
    public static class CharacterStatusManager
    {
        /// <summary>
        /// パーティ内にいるキャラクターのIDのリストです。
        /// </summary>
        public static List<int> partyCharacter;

        /// <summary>
        /// キャラクターのステータスのリストです。
        /// </summary>
        public static List<CharacterStatus> characterStatuses;

        /// <summary>
        /// プレイヤーの所持金です。
        /// </summary>
        public static int partyGold;

        /// <summary>
        /// プレイヤーの所持アイテムのリストです。
        /// </summary>
        public static List<PartyItemInfo> partyItemInfoList;

        /// <summary>
        /// パーティ内のキャラクターのステータスをIDで取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static CharacterStatus GetCharacterStatusById(int characterId)
        {
            return characterStatuses.Find(character => character.characterId == characterId);
        }

        /// <summary>
        /// 対象のキャラクターが倒れたかどうかを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターのID</param>
        public static bool IsCharacterDefeated(int characterId)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            return characterStatus.isDefeated;
        }

        /// <summary>
        /// 全てのキャラクターが倒れたかどうかを取得します。
        /// </summary>
        public static bool IsAllCharacterDefeated()
        {
            bool isAllDefeated = true;
            foreach (int characterId in partyCharacter)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (!characterStatus.isDefeated)
                {
                    isAllDefeated = false;
                    break;
                }
            }
            return isAllDefeated;
        }

        /// <summary>
        /// HPが0でないパーティキャラクターの経験値を増加させます。
        /// </summary>
        /// <param name="exp">増加させる経験値</param>
        public static void IncreaseExp(int exp)
        {
            foreach (var characterId in partyCharacter)
            {
                var characterStatus = GetCharacterStatusById(characterId);
                if (!characterStatus.isDefeated)
                {
                    characterStatus.exp += exp;
                }
            }
        }

        /// <summary>
        /// パーティの所持金を増加させます。
        /// </summary>
        /// <param name="gold">増加させる金額</param>
        public static void IncreaseGold(int gold)
        {
            partyGold += gold;
        }
    }
}