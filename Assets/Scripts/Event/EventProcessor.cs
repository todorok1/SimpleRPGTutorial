using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントの処理を行うクラスです。
    /// </summary>
    public class EventProcessor : MonoBehaviour
    {
        /// <summary>
        /// 現在処理中のイベントです。
        /// </summary>
        EventFileData _currentEventFileData;

        /// <summary>
        /// イベント完了後のコールバック先です。
        /// </summary>
        IEventCallback _callback;

        /// <summary>
        /// イベントの実行を開始します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        /// <param name="callback">イベント完了後のコールバック先</param>
        public void StartEvent(GameObject targetObj, IEventCallback callback)
        {
            _callback = callback;
            CheckEventFile(targetObj);
        }

        /// <summary>
        /// 引数のゲームオブジェクトについて、実行するイベントがあるか確認します。
        /// </summary>
        /// <param name="targetObj">イベントを確認する対象のゲームオブジェクト</param>
        public void CheckEventFile(GameObject targetObj)
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

            _currentEventFileData = eventFileData;
            _currentEventFileData.SetReference(this);
            _currentEventFileData.ExecuteEvent();
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
        }
    }
}