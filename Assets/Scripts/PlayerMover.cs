using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動制御を行うクラスです。
    /// </summary>
    public class PlayerMover : CharacterMover
    {
        void Update()
        {
            CheckMoveInput();
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
    }
}