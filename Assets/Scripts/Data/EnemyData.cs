using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの情報を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/SimpleRpg/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        /// <summary>
        /// 敵キャラクターのIDです。
        /// </summary>
        public int enemyId;

        /// <summary>
        /// 敵キャラクターの名前です。
        /// </summary>
        public string enemyName;

        /// <summary>
        /// 敵キャラクターの画像です。
        /// </summary>
        public Sprite sprite;

        /// <summary>
        /// HPの値です。
        /// </summary>
        public int hp;

        /// <summary>
        /// MPの値です。
        /// </summary>
        public int mp;

        /// <summary>
        /// 力の値です。
        /// </summary>
        public int strength;

        /// <summary>
        /// 身のまもりの値です。
        /// </summary>
        public int guard;

        /// <summary>
        /// 素早さの値です。
        /// </summary>
        public int speed;

        /// <summary>
        /// 経験値の値です。
        /// </summary>
        public int exp;

        /// <summary>
        /// ゴールドの値です。
        /// </summary>
        public int gold;
    }
}