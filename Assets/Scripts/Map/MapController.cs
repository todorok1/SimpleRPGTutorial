using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// マップパーツを制御するクラスです。
    /// </summary>
    public class MapController : MonoBehaviour
    {
        /// <summary>
        /// マップのIDです。
        /// </summary>
        [SerializeField]
        int _mapId;

        /// <summary>
        /// マップのエンカウント設定です。
        /// </summary>
        [SerializeField]
        EncounterData _encounterData;

        /// <summary>
        /// マップのIDです。
        /// </summary>
        public int MapId
        {
            get {return _mapId;}
        }

        /// <summary>
        /// マップの名称です。
        /// </summary>
        [SerializeField]
        string _mapName;

        /// <summary>
        /// マップの名称です。
        /// </summary>
        public string MapName
        {
            get {return _mapName;}
        }

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        /// <summary>
        /// マップの名称です。
        /// </summary>
        public void SetEncounterData(EncounterManager encounterManager)
        {
            encounterManager.SetCurrentEncounterData(_encounterData);
        }
    }
}