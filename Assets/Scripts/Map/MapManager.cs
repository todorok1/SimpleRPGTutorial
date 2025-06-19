using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// マップ機能を管理するクラスです。
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        /// ゲーム開始時に読み込むマップです。
        /// </summary>
        [SerializeField]
        int _gameStartMapId = 1;

        /// <summary>
        /// マップ用ゲームオブジェクトの親オブジェクトです。
        /// </summary>
        [SerializeField]
        Transform _mapParent;

        /// <summary>
        /// 敵キャラクターとのエンカウントを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EncounterManager _encounterManager;

        /// <summary>
        /// Tilemapに関する機能を提供する管理クラスへの参照です。
        /// </summary>
        [SerializeField]
        TilemapManager _tilemapManager;

        /// <summary>
        /// キャラクターの移動を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        EventProcessor _eventProcessor;

        [Header("テスト用設定")]
        /// <summary>
        /// テスト用に表示するマップのIDです。
        /// </summary>
        [SerializeField]
        int _debugTargetMapId = 1;

        /// <summary>
        /// テスト用に表示するマップのPrefabをロードするかどうかのフラグです。
        /// </summary>
        [SerializeField]
        bool _debugLoadTargetMap;

        /// <summary>
        /// 現在表示中のマップの制御クラスです。
        /// </summary>
        MapController _currentMapController;

        /// <summary>
        /// インスタンス化済みのマップの制御クラスのリストです。
        /// </summary>
        List<MapController> _mapControllers = new();

        /// <summary>
        /// 読み込んだマップのPrefabデータの一覧です。
        /// </summary>
        List<GameObject> _mapPrefabs = new();

        /// <summary>
        /// Map用Prefabのファイル名の接頭辞です。
        /// </summary>
        readonly string MapNamePrefix = "Map_";

        void Start()
        {
            LoadMapPrefab();
        }

        /// <summary>
        /// マップ用Prefabをロードします。
        /// </summary>
        public async void LoadMapPrefab()
        {
            AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(AddressablesLabels.Map, null);
            await handle.Task;
            _mapPrefabs = new List<GameObject>(handle.Result);
            handle.Release();

            ShowStartMap();
        }

        /// <summary>
        /// ゲーム開始時のマップを表示します。
        /// </summary>
        void ShowStartMap()
        {
            // ゲーム開始時のマップを表示します。
            ShowMap(_gameStartMapId);
        }

        void Update()
        {
            ShowDebugMap();
        }

        /// <summary>
        /// デバッグ用にマップを表示します。
        /// </summary>
        void ShowDebugMap()
        {
            if (!_debugLoadTargetMap)
            {
                return;
            }

            _debugLoadTargetMap = false;
            ShowMap(_debugTargetMapId);
        }

        /// <summary>
        /// IDからマップのPrefabデータを取得します。
        /// </summary>
        public GameObject GetMapPrefabById(int mapId)
        {
            // 0000のように4桁のID文字列に変換します。
            string mapNamePrefix = MapNamePrefix + mapId.ToString("D4");
            var mapPrefab = _mapPrefabs.Find(prefab => prefab.name.StartsWith(mapNamePrefix));
            return mapPrefab;
        }

        /// <summary>
        /// マップIDに対応するマップを表示します。
        /// </summary>
        public void ShowMap(int mapId)
        {
            _currentMapController = GetTargetMap(mapId);
            if (_currentMapController == null)
            {
                // マップが見つからない場合は、AddressablesからPrefabを取得してインスタンス化します。
                _currentMapController = InstantiateMap(mapId);
            }

            if (_currentMapController != null)
            {
                HideAllMap();
                _currentMapController.gameObject.SetActive(true);
                _currentMapController.SetEncounterData(_encounterManager);
                _currentMapController.SetTilemaps(_tilemapManager);
                _tilemapManager.ResetPositions();
                _characterMoverManager.ResetPositions();

                SimpleLogger.Instance.Log($"マップを表示します。 ID: {mapId} Name: {_currentMapController.MapName}");
                CheckAutoEvent();
            }
        }

        /// <summary>
        /// リスト内のマップを全て非表示にします。
        /// </summary>
        void HideAllMap()
        {
            foreach (var mapController in _mapControllers)
            {
                mapController.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// マップIDに対応するマップを取得します。
        /// </summary>
        MapController GetTargetMap(int mapId)
        {
            MapController mapController = _mapControllers.Find(map => map.MapId == mapId);
            return mapController;
        }

        /// <summary>
        /// マップIDに対応するPrefabをインスタンス化します。
        /// 制御用のMapControllerを返します。
        /// </summary>
        MapController InstantiateMap(int mapId)
        {
            var mapPrefab = GetMapPrefabById(mapId);
            if (mapPrefab != null)
            {
                var mapObject = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity, _mapParent);
                var mapController = mapObject.GetComponent<MapController>();
                _mapControllers.Add(mapController);
                return mapController;
            }
            else
            {
                SimpleLogger.Instance.LogError($"指定したIDのPrefabデータが見つかりませんでした。 ID: {mapId}");
                return null;
            }
        }

        /// <summary>
        /// 現在表示中のマップの制御クラスを取得します。
        /// </summary>
        public MapController GetCurrentMapController()
        {
            return _currentMapController;
        }

        /// <summary>
        /// IDに対応するマップの名前を取得します。
        /// </summary>
        public string GetMapNameFromId(int mapId)
        {
            string mapName = string.Empty;
            var mapPrefab = GetMapPrefabById(mapId);
            if (mapPrefab == null)
            {
                SimpleLogger.Instance.LogWarning($"指定したIDのマップPrefabが見つかりませんでした。 ID: {mapId}");
                return mapName;
            }
            
            var controller = mapPrefab.GetComponent<MapController>();
            if (controller == null)
            {
                SimpleLogger.Instance.LogWarning($"指定したIDのPrefabでMapControllerが見つかりませんでした。 ID: {mapId}");
                return mapName;
            }

            return controller.MapName;
        }

        /// <summary>
        /// マップ内の自動イベントを確認します。
        /// </summary>
        public void CheckAutoEvent()
        {
            SimpleLogger.Instance.Log("CheckAutoEvent()が呼ばれました。");

            if (_currentMapController == null)
            {
                SimpleLogger.Instance.Log("現在のマップコントローラーが設定されていません。");
                return;
            }

            var eventDataList = _currentMapController.GetComponentsInChildren<EventFileData>();
            SimpleLogger.Instance.Log($"eventDataListの数: {eventDataList.Length}");
            foreach (var eventData in eventDataList)
            {
                // 自動イベントを実行します。
                var eventQueue = new EventQueue
                {
                    targetObj = eventData.gameObject,
                    rpgEventTrigger = RpgEventTrigger.Auto,
                    callback = null
                };
                _eventProcessor.AddQueue(eventQueue);
            }
            _eventProcessor.StartEvent();
        }
    }
}