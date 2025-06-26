using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動制御を行うクラスです。
    /// </summary>
    public class PlayerMover : CharacterMover
    {
        /// <summary>
        /// 操作キャラの移動に伴うイベントの確認を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        PlayerEventChecker _playerEventChecker;

        /// <summary>
        /// 敵キャラクターとのエンカウントを管理するクラスへの参照です。
        /// </summary>
        EncounterManager _encounterManager;

        protected override void Start()
        {
            base.Start();
            _playerEventChecker.SetUpReference(this);
        }

        void Update()
        {
            CheckMoveInput();
            _playerEventChecker.CheckEventInput();
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
            if (_playerEventChecker.CheckOnTileEvent())
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
    }
}