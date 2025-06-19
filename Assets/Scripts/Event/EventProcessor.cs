using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントの処理を行うクラスです。
    /// </summary>
    public class EventProcessor : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// 現在処理中のイベントです。
        /// </summary>
        EventFileData _currentEventFileData;

        /// <summary>
        /// イベント完了後のコールバック先です。
        /// </summary>
        IEventCallback _callback;

        /// <summary>
        /// 実行するイベントのキューです。
        /// </summary>
        Queue<EventQueue> _eventQueue = new();

        /// <summary>
        /// キューにイベントを追加します。
        /// </summary>
        /// <param name="eventQueue">追加するイベントキュー</param>
        public void AddQueue(EventQueue eventQueue)
        {
            _eventQueue.Enqueue(eventQueue);
        }

        /// <summary>
        /// キューに追加されたイベントを開始します。
        /// </summary>
        public void StartEvent()
        {
            if (_eventQueue.Count == 0)
            {
                SimpleLogger.Instance.Log("イベントキューが空のため処理を抜けます。");
                return;
            }

            var eventQueue = _eventQueue.Dequeue();
            if (eventQueue == null)
            {
                SimpleLogger.Instance.Log("イベントキューが空です。");
                return;
            }
            ExecuteEvent(eventQueue.targetObj, eventQueue.rpgEventTrigger, eventQueue.callback);
        }

        /// <summary>
        /// イベントの実行を開始します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        /// <param name="callback">イベント完了後のコールバック先</param>
        public void ExecuteEvent(GameObject targetObj, RpgEventTrigger rpgEventTrigger, IEventCallback callback)
        {
            _callback = callback;
            CheckEventFile(targetObj, rpgEventTrigger);
        }

        /// <summary>
        /// 引数のゲームオブジェクトについて、実行するイベントがあるか確認します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        public void CheckEventFile(GameObject targetObj, RpgEventTrigger rpgEventTrigger)
        {
            SimpleLogger.Instance.Log($"CheckEventFile()が呼ばれました。");
            var eventFileData = targetObj.GetComponent<EventFileData>();
            if (eventFileData == null)
            {
                eventFileData = targetObj.GetComponentInChildren<EventFileData>();
                if (eventFileData == null)
                {
                    SimpleLogger.Instance.LogWarning($"イベントファイルデータが見つかりませんでした。対象のゲームオブジェクト : {targetObj.name}");
                    return;
                }
            }

            GameStateManager.ChangeToEvent();
            _characterMoverManager.StopCharacterMover();
            _currentEventFileData = eventFileData;
            _currentEventFileData.SetReference(this);
            _currentEventFileData.ExecuteEvent(rpgEventTrigger);
        }

        /// <summary>
        /// イベントの処理が終了した場合のコールバックです。
        /// </summary>
        public void OnEventFinished()
        {
            // イベントの処理が終了した後の処理をここに記述します。
            SimpleLogger.Instance.Log("イベントの処理が完了しました。");
            _currentEventFileData = null;

            if (_callback != null)
            {
                _callback.OnFinishedEvent();
            }
            _callback = null;

            if (_eventQueue.Count > 0)
            {
                // キューにイベントが残っている場合は次のイベントを開始します。
                StartEvent();
                return;
            }

            // すべてのイベントが完了した場合は、ゲーム状態を移動に戻します。
            GameStateManager.ChangeToMoving();
            _characterMoverManager.ResumeCharacterMover();
        }
    }
}