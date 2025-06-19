using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントのページを制御するクラスです。
    /// </summary>
    public class EventPage : MonoBehaviour
    {
        /// <summary>
        /// イベントのページの条件を保持するクラスのリストです。
        /// </summary>
        [SerializeField]
        List<EventPageConditionBase> _conditions;

        /// <summary>
        /// イベントのページのトリガーです。
        /// </summary>
        [SerializeField]
        RpgEventTrigger _eventTrigger;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        EventProcessor _eventProcessor;

        /// <summary>
        /// このページ内のイベント処理のリストです。
        /// </summary>
        List<EventProcessBase> _eventProcessList;

        /// <summary>
        /// 現在のイベント処理のインデックスです。
        /// </summary>
        int _currentEventIndex;

        /// <summary>
        /// イベントの条件を全て満たしているか確認します。
        /// </summary>
        public bool IsMatchedConditions()
        {
            foreach (var condition in _conditions)
            {
                // ひとつでも条件が満たされていない場合はfalseを返します。
                if (!condition.CheckCondition())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// イベントのトリガーが一致しているか確認します。
        /// </summary>
        public bool IsMatchedTrigger(RpgEventTrigger rpgEventTrigger)
        {
            return _eventTrigger == rpgEventTrigger;
        }

        /// <summary>
        /// このページのイベント処理を取得します。
        /// </summary>
        public void SetUpProcesses()
        {
            var processes = GetComponentsInChildren<EventProcessBase>();
            _eventProcessList = new List<EventProcessBase>(processes);
        }

        /// <summary>
        /// イベントの処理を開始します。
        /// </summary>
        /// <param name="eventProcessor">イベントの処理を行うクラスへの参照です。</param>
        public void StartEvent(EventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
            _currentEventIndex = 0;
            SetUpProcesses();
            if (_eventProcessList == null || _eventProcessList.Count == 0)
            {
                SimpleLogger.Instance.Log("このページに対応するEventProcessがありませんでした。");
                OnFinishedEventPage();
                return;
            }

            ExecuteEventProcess();
        }

        /// <summary>
        /// インデックスに応じてイベントの処理を実行します。
        /// </summary>
        public void ExecuteEventProcess()
        {
            var targetProcess = _eventProcessList[_currentEventIndex];
            if (targetProcess == null)
            {
                SimpleLogger.Instance.LogWarning($"対象のインデックスのイベント処理が見つかりませんでした。_currentEventIndex : {_currentEventIndex}");
                OnFinishedEventPage();
                return;
            }

            targetProcess.SetEventPageReference(this);
            targetProcess.Execute();
        }

        /// <summary>
        /// イベントの個別の処理が完了した時の処理です。
        /// </summary>
        public void OnFinishedEventProcess()
        {
            _currentEventIndex++;
            if (_currentEventIndex >= _eventProcessList.Count)
            {
                // 全てのイベント処理が完了した場合は、イベントページを終了します。
                OnFinishedEventPage();
                return;
            }

            ExecuteEventProcess();
        }

        /// <summary>
        /// イベントページ内の処理が完了した時の処理です。
        /// </summary>
        public void OnFinishedEventPage()
        {
            _eventProcessor.OnEventFinished();
        }
    }
}