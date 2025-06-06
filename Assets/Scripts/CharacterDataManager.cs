using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内の味方キャラクターのデータを管理するクラスです。
    /// </summary>
    public static class CharacterDataManager
    {
        /// <summary>
        /// 読み込んだキャラクターの経験値表の一覧です。
        /// </summary>
        static List<ExpTable> _expTables = new();

        /// <summary>
        /// 読み込んだキャラクターのパラメータ表の一覧です。
        /// </summary>
        static List<ParameterTable> _parameterTables = new();

        /// <summary>
        /// 読み込んだキャラクターのデータの一覧です。
        /// </summary>
        static List<CharacterData> _characterDataList = new();

        /// <summary>
        /// 経験値表のデータをロードします。
        /// </summary>
        public static async void LoadExpTables()
        {
            AsyncOperationHandle<IList<ExpTable>> handle = Addressables.LoadAssetsAsync<ExpTable>(AddressablesLabels.ExpTable, null);
            await handle.Task;
            _expTables = new List<ExpTable>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// 経験値表のデータを取得します。
        /// </summary>
        public static ExpTable GetExpTable()
        {
            ExpTable expTable = null;
            if (_expTables.Count > 0)
            {
                expTable = _expTables[0];
            }
            return expTable;
        }

        /// <summary>
        /// パラメータ表のデータをロードします。
        /// </summary>
        public static async void LoadParameterTables()
        {
            AsyncOperationHandle<IList<ParameterTable>> handle = Addressables.LoadAssetsAsync<ParameterTable>(AddressablesLabels.ParameterTable, null);
            await handle.Task;
            _parameterTables = new List<ParameterTable>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// IDからパラメータ表のデータを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static ParameterTable GetParameterTable(int characterId)
        {
            return _parameterTables.Find(parameterTable => parameterTable.characterId == characterId);
        }

        /// <summary>
        /// キャラクターの定義データをロードします。
        /// </summary>
        public static async void LoadCharacterData()
        {
            AsyncOperationHandle<IList<CharacterData>> handle = Addressables.LoadAssetsAsync<CharacterData>(AddressablesLabels.Character, null);
            await handle.Task;
            _characterDataList = new List<CharacterData>(handle.Result);
            handle.Release();
        }

        /// <summary>
        /// キャラクターのIDからキャラクターの定義データを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static CharacterData GetCharacterData(int characterId)
        {
            return _characterDataList.Find(character => character.characterId == characterId);
        }

        /// <summary>
        /// キャラクターのIDからキャラクターの名前を取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static string GetCharacterName(int characterId)
        {
            var characterData = GetCharacterData(characterId);
            return characterData.characterName;
        }

        /// <summary>
        /// キャラクターのIDから覚える魔法データを取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        public static List<CharacterMagicRecord> GetCharacterMagicList(int characterId)
        {
            var characterData = _characterDataList.Find(character => character.characterId == characterId);
            return characterData.characterMagicRecords;
        }

        /// <summary>
        /// キャラクターのIDとレベルから現在覚えられる魔法データ一覧を取得します。
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <param name="level">キャラクターのレベル</param>
        public static List<MagicData> GetLearnableMagic(int characterId, int level)
        {
            var magicList = GetCharacterMagicList(characterId);
            var records = magicList.Where(x => x.level <= level);
            List<MagicData> magicDataList = new();
            foreach (var record in records)
            {
                var magicData = MagicDataManager.GetMagicDataById(record.magicId);
                if (magicData == null)
                {
                    SimpleLogger.Instance.LogWarning($"魔法データが見つかりませんでした。 ID: {record.magicId}");
                    continue;
                }
                magicDataList.Add(magicData);
            }
            return magicDataList;
        }
    }
}