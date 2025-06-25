using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 条件に応じたイベントの画像を制御するクラスです。
    /// </summary>
    public class EventGraphicController : MonoBehaviour
    {
        /// <summary>
        /// イベントページと画像の対応レコードです。
        /// </summary>
        [SerializeField]
        List<EventGraphicRecord> _eventGraphicRecords;

        /// <summary>
        /// 変更する対象のSpriteRendererです。
        /// </summary>
        [SerializeField]
        SpriteRenderer _spriteRenderer;

        /// <summary>
        /// 変更する対象のAnimatorです。
        /// </summary>
        [SerializeField]
        Animator _animator;

        /// <summary>
        /// 条件に合致するイベントページに対応する画像を表示します。
        /// </summary>
        public void SetEventGraphic(EventPage eventPage)
        {
            SimpleLogger.Instance.Log("SetEventGraphic()が呼ばれました。");
            if (_eventGraphicRecords == null)
            {
                return;
            }

            // 対応するイベントページをグラフィック設定から探します。
            foreach (var record in _eventGraphicRecords)
            {
                if (record == null || record.eventPages == null || record.eventPages.Count == 0)
                {
                    SimpleLogger.Instance.Log("レコードにイベントページが設定されていないためスキップします。");
                    continue;
                }

                SimpleLogger.Instance.Log($"record.name : {record.name}, record.eventPages.Contains(eventPage) : {record.eventPages.Contains(eventPage)}");
                if (record.eventPages.Contains(eventPage))
                {
                    // スプライトを変更します。
                    if (record.sprite != null && _spriteRenderer != null)
                    {
                        _spriteRenderer.sprite = record.sprite;
                    }

                    // アニメーターを変更します。
                    if (_animator != null)
                    {
                        if (record.animatorController != null)
                        {
                            _animator.runtimeAnimatorController = record.animatorController;
                        }

                        _animator.enabled = !record.isStopAnimator;
                    }

                    // // ゲームオブジェクトの表示状態を変更します。
                    // if (record.topGameObject != null)
                    // {
                    //     record.topGameObject.SetActive(record.isVisible);
                    // }
                    return;
                }
            }
        }
    }
}