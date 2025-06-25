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
            // イベント中など、移動後の処理を行わない場合は処理を抜けます。
            if (!_isCheckPostMove)
            {
                return;
            }

            // 移動後のマスにイベントがあるかどうかを確認します。
            if (CheckOnTileEvent())
            {
                return;
            }

            // イベントがない場合は、エンカウントの確認を行います。
            CheckEncounter();
        }

        /// <summary>
        /// エンカウントが発生するかどうかを確認します。
        /// </summary>
        void CheckEncounter()
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
                // SimpleLogger.Instance.Log("GameStateがMovingではないので処理を抜けます。");
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
            SimpleLogger.Instance.Log($"hitObj.name : {hitObj.name}");

            // 移動用の制御クラスがアタッチされている場合はキャッシュします。
            _eventTargetMover = hitObj.GetComponent<CharacterMover>();

            // イベントファイルをイベント処理のクラスに渡します。
            StartEvent(hitObj, RpgEventTrigger.ConfirmButton);
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
        /// 引数のColliderのリストからゲームオブジェクトを取得します。
        /// </summary>
        /// <param name="colliders">Collider2Dのリスト</param>
        GameObject GetGameObjectFromColliders(List<Collider2D> colliders)
        {
            if (colliders == null || colliders.Count == 0)
            {
                return null;
            }

            // 最初のColliderのゲームオブジェクトを返します。
            GameObject targetObj = colliders[0].gameObject;
            return targetObj;
        }

        /// <summary>
        /// イベントが終了した時に呼ばれるコールバックです。
        /// </summary>
        public void OnFinishedEvent()
        {
            SetEventMoverDirectionPrevious();
            _eventTargetMover = null;
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
            _eventTargetMover.SetCharacterDirection((MoveAnimationDirection)oppsiteDirection);
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
            _eventTargetMover.SetCharacterDirection(_eventTargetMover.AnimationDirection);
        }

        /// <summary>
        /// タイル上で重なったイベントを確認します。
        /// </summary>
        bool CheckOnTileEvent()
        {
            // 重なったゲームオブジェクトを確認します。
            List<Collider2D> colliders = new List<Collider2D>();
            var otherColliders = _boxCollider2d.Overlap(colliders);

            // 対象のゲームオブジェクトを取得します。
            var eventObj = GetGameObjectFromColliders(colliders);
            if (eventObj == null)
            {
                return false;
            }

            // 対象のゲームオブジェクトのマップ上の位置を確認します。
            var eventPos = _tilemapManager.GetPositionOnTilemap(eventObj.transform.position);
            if (eventPos != _posOnTile)
            {
                SimpleLogger.Instance.Log("タイル上の位置が異なるので処理を抜ける。");
                return false;
            }

            // イベントファイルをイベント処理のクラスに渡します。
            StartEvent(eventObj, RpgEventTrigger.OnTile);
            return true;
        }

        /// <summary>
        /// イベントの処理を開始します。
        /// </summary>
        void StartEvent(GameObject eventObj, RpgEventTrigger rpgEventTrigger)
        {
            // イベントファイルをイベント処理のクラスに渡します。
            GetReference();
            if (_eventProcessor == null)
            {
                SimpleLogger.Instance.LogError("EventProcessorが見つかりませんでした。");
                return;
            }

            // イベントの対象となるキャラクターの向きを設定します。
            SetEventMoverDirectionToPlayer();

            // イベントの開始を通知します。
            SimpleLogger.Instance.Log("イベントの開始を通知します。");
            var eventQueue = new EventQueue
            {
                targetObj = eventObj,
                rpgEventTrigger = rpgEventTrigger,
                callback = this
            };
            _eventProcessor.AddQueue(eventQueue);
            _eventProcessor.StartEvent();
        }
    }
}