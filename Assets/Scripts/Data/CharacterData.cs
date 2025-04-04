using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターに関する設定を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/SimpleRpg/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        /// <summary>
        /// キャラクターのIDです。
        /// </summary>
        public int characterId;

        /// <summary>
        /// キャラクターの名前です。
        /// </summary>
        public string characterName;

        /// <summary>
        /// レベルと覚える魔法の対応レコードのリストです。
        /// </summary>
        public List<CharacterMagicRecord> characterMagicRecords;
    }
}