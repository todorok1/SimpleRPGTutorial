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
        /// 最初に実行するイベントです。
        /// </summary>
        [SerializeField]
        EventProcessBase _startProcess;

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
        /// 実行対象のイベントです。
        /// </summary>
        EventProcessBase _targetEventProcess;

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
            _targetEventProcess = _startProcess;
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
        /// 対象のイベントの処理を実行します。
        /// </summary>
        public void ExecuteEventProcess()
        {
            if (_targetEventProcess == null)
            {
                SimpleLogger.Instance.LogWarning($"実行対象のイベントプロセスがnullです。");
                OnFinishedEventPage();
                return;
            }

            _targetEventProcess.SetEventPageReference(this);
            _targetEventProcess.Execute();
        }

        /// <summary>
        /// イベントの個別の処理が完了した時の処理です。
        /// </summary>
        public void OnFinishedEventProcess(EventProcessBase nextProcess)
        {
            if (nextProcess == null)
            {
                // 全てのイベント処理が完了した場合は、イベントページを終了します。
                OnFinishedEventPage();
                return;
            }

            _targetEventProcess = nextProcess;
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