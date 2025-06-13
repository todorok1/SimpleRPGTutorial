using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動制御を行うクラスです。
    /// </summary>
    public class PlayerMover : CharacterMover, IEventCallback
    {
        /// <summary>
        /// 敵キャラクターとのエンカウントを管理するクラスへの参照です。
        /// </summary>
        EncounterManager _encounterManager;

        /// <summary>
        /// イベントの処理を行うクラスへの参照です。
        /// </summary>
        EventProcessor _eventProcessor;

        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// イベントを実行しているキャラクターの移動用クラスへの参照です。
        /// </summary>
        CharacterMover _eventTargetMover;

        void Update()
        {
            CheckMoveInput();
            CheckEventInput();
        }

        /// <summary>
        /// 参照を取得します。
        /// </summary>
        void GetReference()
        {
            if (_encounterManager == null)
            {
                _encounterManager = FindAnyObjectByType<EncounterManager>();
            }

            if (_eventProcessor == null)
            {
                _eventProcessor = FindAnyObjectByType<EventProcessor>();
            }

            if (_characterMoverManager == null)
            {
                _characterMoverManager = FindAnyObjectByType<CharacterMoverManager>();
            }
        }

        /// <summary>
        /// キー入力を確認します。
        /// </summary>
        void CheckMoveInput()
        {
            // 既に移動中の場合は移動キーの入力を確認せず抜けます。
            if (_isMoving)
            {
                return;
            }

            // 移動のポーズフラグがtrueなら処理を抜けます。
            if (_isMovingPaused)
            {
                return;
            }

            // 斜め移動は行わないため、上下左右のいずれかを移動対象とします。
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _animationDirection = MoveAnimationDirection.Front;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _animationDirection = MoveAnimationDirection.Right;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                _animationDirection = MoveAnimationDirection.Back;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                _animationDirection = MoveAnimationDirection.Left;
            }
            else
            {
                // 移動キーが押されていない場合は処理を抜けます。
                return;
            }

            var moveDirection = GetMoveDirection(_animationDirection);
            MoveCharacter(moveDirection, _animationDirection);
        }

        /// <summary>
        /// キャラクター移動後の処理です。
        /// </summary>
        protected override void PostMove()
        {
            GetReference();
            if (_encounterManager != null)
            {
                // エンカウントの確認を行います。
                _encounterManager.CheckEncounter();
            }
            else
            {
                SimpleLogger.Instance.LogError("EncounterManagerが見つかりませんでした。");
            }
        }

        /// <summary>
        /// イベントを開始するキー入力を確認します。
        /// </summary>
        void CheckEventInput()
        {
            // 移動フェーズ以外は処理を抜けます。
            if (GameStateManager.CurrentState != GameState.Moving)
            {
                SimpleLogger.Instance.Log("GameStateがMovingではないので処理を抜けます。");
                return;
            }

            // このキャラクターが移動中の場合、または移動が停止している場合は処理を抜けます。
            if (_isMoving || _isMovingPaused)
            {
                // SimpleLogger.Instance.Log($"移動中または移動できない状態のため処理を抜けます。_isMoving : {_isMoving}, _isMovingPaused : {_isMovingPaused}");
                return;
            }

            // イベントの開始キーが押された場合は、イベントを開始します。
            if (InputGameKey.ConfirmButton())
            {
                CheckEvent();
            }
        }

        /// <summary>
        /// イベントを開始するキー入力を確認します。
        /// </summary>
        void CheckEvent()
        {
            SimpleLogger.Instance.Log("CheckEvent()が呼ばれました。");

            // 目の前にいるゲームオブジェクトを確認します。
            var moveDirection = GetMoveDirection(_animationDirection);
            SimpleLogger.Instance.Log("moveDirection : " + moveDirection);
            var raycastHits = GetOtherCharacter(moveDirection);

            // 対象のゲームオブジェクトを取得します。
            var hitObj = GetGameObjectFromRaycastHits(raycastHits);
            if (hitObj == null)
            {
                SimpleLogger.Instance.Log("hitObjがnullなので処理を抜ける。");
                return;
            }

            // 移動用の制御クラスがアタッチされている場合はキャッシュします。
            _eventTargetMover = hitObj.GetComponent<CharacterMover>();

            // イベントファイルをイベント処理のクラスに渡します。
            GetReference();
            if (_eventProcessor != null)
            {
                // 他のキャラクターの動作を止めます。
                StopCharacterMove();

                // イベントの対象となるキャラクターの向きを設定します。
                SetEventMoverDirectionToPlayer();

                // イベントの開始を通知します。
                SimpleLogger.Instance.Log("イベントの開始を通知します。");
                _eventProcessor.StartEvent(hitObj, this);
            }
            else
            {
                SimpleLogger.Instance.LogError("EventProcessorが見つかりませんでした。");
            }
        }

        /// <summary>
        /// 引数のレイキャストの結果からゲームオブジェクトを取得します。
        /// </summary>
        /// <param name="raycastHits">レイキャストの結果のリスト</param>
        GameObject GetGameObjectFromRaycastHits(List<RaycastHit2D> raycastHits)
        {
            if (raycastHits == null || raycastHits.Count == 0)
            {
                return null;
            }

            GameObject targetObj = raycastHits[0].transform.gameObject;
            return targetObj;
        }

        /// <summary>
        /// イベントが終了した時に呼ばれるコールバックです。
        /// </summary>
        public void OnFinishedEvent()
        {
            ResumeCharacterMove();
            SetEventMoverDirectionPrevious();
            _eventTargetMover = null;
        }

        /// <summary>
        /// キャラクターの動作を停止します。
        /// </summary>
        void StopCharacterMove()
        {
            GetReference();
            if (_characterMoverManager != null)
            {
                // 他のキャラクターの動作を止めます。
                _characterMoverManager.StopCharacterMover();
            }
            else
            {
                SimpleLogger.Instance.LogError("CharacterMoverManagerが見つかりませんでした。");
            }
        }

        /// <summary>
        /// キャラクターの動作を再開します。
        /// </summary>
        void ResumeCharacterMove()
        {
            GetReference();
            if (_characterMoverManager != null)
            {
                // 他のキャラクターの動作を再開します。
                _characterMoverManager.ResumeCharacterMover();
            }
            else
            {
                SimpleLogger.Instance.LogError("CharacterMoverManagerが見つかりませんでした。");
            }
        }

        /// <summary>
        /// イベントの実行対象となるキャラクターの向きを変更します。
        /// </summary>
        void SetEventMoverDirectionToPlayer()
        {
            if (_eventTargetMover == null)
            {
                SimpleLogger.Instance.Log($"イベントの対象となるキャラクターの移動クラスが見つかりませんでした。");
                return;
            }

            // 主人公の向きの反対を取得します。
            int playerDirection = (int)_animationDirection;
            int oppsiteDirection = (playerDirection + 2) % 4;

            // イベントの対象となるキャラクターの向きを設定します。
            _eventTargetMover.UpdateCharacterDirection((MoveAnimationDirection)oppsiteDirection);
        }

        /// <summary>
        /// イベントの実行対象となるキャラクターの向きを変更します。
        /// </summary>
        void SetEventMoverDirectionPrevious()
        {
            if (_eventTargetMover == null)
            {
                SimpleLogger.Instance.Log($"イベントの対象となるキャラクターの移動クラスが見つかりませんでした。");
                return;
            }

            // 元の向きに設定します。
            _eventTargetMover.UpdateCharacterDirection(_eventTargetMover.AnimationDirection);
        }
    }
}