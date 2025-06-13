using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// イベントの個別処理を行うクラスのベースクラスです。
    /// </summary>
    public class EventProcessBase : MonoBehaviour
    {
        /// <summary>
        /// イベントページへの参照です。
        /// </summary>
        protected EventPage _eventPage;

        /// <summary>
        /// イベントページへの参照をセットします。
        /// </summary>
        public virtual void SetEventPageReference(EventPage eventPage)
        {
            _eventPage = eventPage;
        }

        /// <summary>
        /// イベントの処理を実行します。
        /// </summary>
        public virtual void Execute()
        {

        }

        /// <summary>
        /// 次のイベント処理を呼び出します。
        /// </summary>
        public virtual void CallNextProcess()
        {
            if (_eventPage == null)
            {
                SimpleLogger.Instance.LogWarning("イベントページへの参照が設定されていません。");
                return;
            }

            _eventPage.OnFinishedEventProcess();
        }
    }
}