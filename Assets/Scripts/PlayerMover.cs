using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動制御を行うクラスです。
    /// </summary>
    public class PlayerMover : CharacterMover
    {
        /// <summary>
        /// 敵キャラクターとのエンカウントを管理するクラスへの参照です。
        /// </summary>
        EncounterManager _encounterManager;

        void Update()
        {
            CheckMoveInput();
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

            var moveDirection = Vector2Int.zero;
            MoveAnimationDirection animDirection = MoveAnimationDirection.Front;

            // 斜め移動は行わないため、上下左右のいずれかを移動対象とします。
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection = Vector2Int.down;
                animDirection = MoveAnimationDirection.Front;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection = Vector2Int.right;
                animDirection = MoveAnimationDirection.Right;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection = Vector2Int.up;
                animDirection = MoveAnimationDirection.Back;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection = Vector2Int.left;
                animDirection = MoveAnimationDirection.Left;
            }

            MoveCharacter(moveDirection, animDirection);
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
    }
}