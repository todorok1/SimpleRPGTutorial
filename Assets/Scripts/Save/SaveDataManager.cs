using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータの管理を行うクラスです。
    /// </summary>
    [Serializable]
    public class SaveDataManager : MonoBehaviour
    {
        /// <summary>
        /// セーブデータとステータス情報をやりとりするクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveInfoStatusController _saveInfoStatusController;

        /// <summary>
        /// セーブデータとマップ情報をやりとりするクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveInfoMapController _saveInfoMapController;

        /// <summary>
        /// セーブデータとフラグ情報をやりとりするクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveInfoFlagController _saveInfoFlagController;

        /// <summary>
        /// 読み込んだセーブファイルのキャッシュです。
        /// </summary>
        SaveFile _saveFile;

        /// <summary>
        /// セーブファイルを初期化します。
        /// </summary>
        public void InitializeSaveFile()
        {
            _saveFile = new()
            {
                saveSlots = new List<SaveSlot>()
            };
        }

        /// <summary>
        /// ファイルをディスクからロードします。
        /// </summary>
        public void LoadFile()
        {
            // セーブファイルのパスを取得し、存在しない場合はフィールドを初期化します。
            string path = SaveDataUtil.GetSaveFilePath();
            if (!File.Exists(path))
            {
                SimpleLogger.Instance.Log($"指定したパスにデータが存在しないため、セーブファイルを初期化します。 path : {path}");
                InitializeSaveFile();
                return;
            }

            try
            {
                string loadedText = File.ReadAllText(path);
                _saveFile = JsonUtility.FromJson<SaveFile>(loadedText);
                SimpleLogger.Instance.Log($"loadedText : {loadedText}");
            }
            catch (Exception e)
            {
                SimpleLogger.Instance.LogError($"ロードに失敗しました。 エラー内容 : {e}");
                InitializeSaveFile();
            }
        }

        /// <summary>
        /// ロードした情報について、指定したセーブ枠の情報をメモリに読み込みます。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        public void SetLoadedData(int slotId)
        {
            if (!SaveDataUtil.IsValidSlotId(slotId))
            {
                SimpleLogger.Instance.LogError($"ロードに失敗しました。セーブ枠のIDが無効です。 ID : {slotId}");
                return;
            }

            if (_saveFile == null)
            {
                SimpleLogger.Instance.LogWarning($"ロードに失敗しました。読み込んだセーブファイルが存在しません。");
                return;
            }

            if (_saveFile.saveSlots == null)
            {
                SimpleLogger.Instance.LogWarning($"ロードに失敗しました。セーブファイルのセーブ枠が存在しません。");
                return;
            }

            var saveSlot = _saveFile.saveSlots.Find(slot => slot.slotId == slotId);
            if (saveSlot == null)
            {
                SimpleLogger.Instance.LogWarning($"ロードに失敗しました。IDに対応するデータが存在しませんでした。 ID : {slotId}");
                return;
            }

            // セーブ枠の情報をメモリに読み込みます。
            _saveInfoStatusController.SetSaveInfoStatus(saveSlot.saveInfoStatus);
            _saveInfoMapController.SetSaveInfoMap(saveSlot.saveInfoMap);
            _saveInfoFlagController.SetSaveInfoFlag(saveSlot.saveInfoFlag);
        }

        /// <summary>
        /// ゲーム内の実行中データをセーブファイルに保存します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        public void SaveDataToFile(int slotId)
        {
            if (!SaveDataUtil.IsValidSlotId(slotId))
            {
                SimpleLogger.Instance.LogError($"セーブに失敗しました。セーブ枠のIDが無効です。 ID : {slotId}");
                return;
            }

            UpdateSlotData(slotId);
            var json = JsonUtility.ToJson(_saveFile);
            var path = SaveDataUtil.GetSaveFilePath();
            try
            {
                File.WriteAllText(path, json);
                SimpleLogger.Instance.Log($"セーブしたjson : {json}");
            }
            catch (Exception e)
            {
                SimpleLogger.Instance.LogError($"セーブに失敗しました。エラー内容 : {e}");
                return;
            }
        }

        /// <summary>
        /// 指定した枠のデータをメモリ上から取得します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        void UpdateSlotData(int slotId)
        {
            var saveInfoStatus = _saveInfoStatusController.GetSaveInfoStatus();
            var saveInfoMap = _saveInfoMapController.GetSaveInfoMap();
            var saveInfoFlag = _saveInfoFlagController.GetSaveInfoFlag();

            if (_saveFile == null)
            {
                InitializeSaveFile();
            }

            if (_saveFile.saveSlots == null)
            {
                InitializeSaveFile();
            }

            var saveSlot = _saveFile.saveSlots.Find(slot => slot.slotId == slotId);
            if (saveSlot == null)
            {
                saveSlot = GetSlotData(slotId);
                _saveFile.saveSlots.Add(saveSlot);
            }
            else
            {
                saveSlot.saveInfoStatus = saveInfoStatus;
                saveSlot.saveInfoMap = saveInfoMap;
                saveSlot.saveInfoFlag = saveInfoFlag;
            }
        }

        /// <summary>
        /// 枠の情報を取得します。
        /// </summary>
        /// <param name="slotId">セーブする枠のID</param>
        SaveSlot GetSlotData(int slotId)
        {
            var saveInfoStatus = _saveInfoStatusController.GetSaveInfoStatus();
            var saveInfoMap = _saveInfoMapController.GetSaveInfoMap();
            var saveInfoFlag = _saveInfoFlagController.GetSaveInfoFlag();

            SaveSlot saveSlot = new()
            {
                slotId = slotId,
                saveInfoStatus = saveInfoStatus,
                saveInfoMap = saveInfoMap,
                saveInfoFlag = saveInfoFlag
            };
            return saveSlot;
        }
    }
}