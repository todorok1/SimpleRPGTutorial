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
        /// フラグの状態を保持するクラスへの参照です。
        /// </summary>
        [SerializeField]
        FlagManager _flagManager;

        /// <summary>
        /// セーブデータ用のフラグ情報を返します。
        /// </summary>
        public SaveInfoFlag GetSaveInfoFlag()
        {
            SaveInfoFlag saveInfoFlag = new()
            {
                flagStates = _flagManager.GetFlagStateList()
            };
            return saveInfoFlag;
        }

        /// <summary>
        /// セーブデータから読み取ったフラグ情報をセットします。
        /// </summary>
        /// <param name="saveInfoFlag">フラグ情報</param>
        public void SetSaveInfoFlag(SaveInfoFlag saveInfoFlag)
        {
            _flagManager.InitializeFlagList();
            if (saveInfoFlag == null)
            {
                SimpleLogger.Instance.LogWarning("セーブデータ内のフラグ情報がnullです。");
                return;
            }

            if (saveInfoFlag.flagStates == null)
            {
                SimpleLogger.Instance.LogWarning("フラグ情報のフラグリストがnullです。");
                return;
            }

            foreach (var flagState in saveInfoFlag.flagStates)
            {
                if (flagState == null || string.IsNullOrEmpty(flagState.flagName))
                {
                    SimpleLogger.Instance.LogWarning("フラグ情報のフラグ名がnullまたは空です。");
                    continue;
                }

                _flagManager.SetFlagState(flagState.flagName, flagState.state);
            }
        }
    }
}