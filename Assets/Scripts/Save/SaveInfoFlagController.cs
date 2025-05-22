using UnityEngine;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータとフラグ情報をやりとりするクラスです。
    /// </summary>
    [Serializable]
    public class SaveInfoFlagController : MonoBehaviour
    {
        /// <summary>
        /// セーブデータ用のフラグ情報を返します。
        /// </summary>
        public SaveInfoFlag GetSaveInfoFlag()
        {
            SaveInfoFlag saveInfoFlag = new();
            return saveInfoFlag;
        }

        /// <summary>
        /// セーブデータから読み取ったフラグ情報をセットします。
        /// </summary>
        /// <param name="saveInfoFlag">フラグ情報</param>
        public void SetSaveInfoFlag(SaveInfoFlag saveInfoFlag)
        {
            
        }
    }
}