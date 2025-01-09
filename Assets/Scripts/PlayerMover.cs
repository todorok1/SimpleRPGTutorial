using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動制御を行うクラスです。
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        /// <summary>
        /// Tilemapを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TilemapManager _tilemapManager;

        /// <summary>
        /// キャラクターの移動にかかる時間です。
        /// </summary>
        [SerializeField]
        float _moveTime = 0.25f;

        /// <summary>
        /// 移動するキャラクターのAnimatorです。
        /// </summary>
        Animator _animator;

        /// <summary>
        /// Tilemap上における現在の論理的な座標です。
        /// </summary>
        Vector3Int _posOnTile = Vector3Int.zero;

        /// <summary>
        /// 移動中かどうかのフラグです。
        /// </summary>
        bool _isMoving;

        void Start()
        {
            CheckComponents();
            GetCurrentPositionOnTilemap();
        }

        void Update()
        {
            CheckMoveInput();
        }

        /// <summary>
        /// 必要なコンポーネントへの参照を確認します。
        /// </summary>
        void CheckComponents()
        {
            if (_animator == null)
            {
                _animator = gameObject.GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Tilemap上の論理的な座標を取得します。
        /// </summary>
        void GetCurrentPositionOnTilemap()
        {
            if (_tilemapManager == null)
            {
                return;
            }

            _posOnTile = _tilemapManager.GetPositionOnTilemap(gameObject.transform.position);
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

            MovePlayer(moveDirection, animDirection);
        }

        /// <summary>
        /// 操作キャラクターを移動させます。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="animDirection">アニメーションの方向</param>
        void MovePlayer(Vector2Int moveDirection, MoveAnimationDirection animDirection)
        {
            if (moveDirection == Vector2Int.zero)
            {
                return;
            }

            // 移動方向に応じたアニメーションに切り替えます。
            if (_animator != null)
            {
                _animator.SetInteger(AnimationSettings.DirectionParameterName, (int)animDirection);
            }

            // 移動できるかの確認を行います。
            var targetPos = _posOnTile + (Vector3Int)moveDirection;
            bool canMove = _tilemapManager.CanEntryTile(targetPos);
            if (!canMove)
            {
                return;
            }

            _isMoving = true;

            // 論理的な座標から移動先の座標を計算し、ワールド座標に変換します。
            _posOnTile = targetPos;
            var targetWorldPos = _tilemapManager.GetWorldPosition(_posOnTile);

            // コルーチンを使ってキャラクターのゲームオブジェクトを移動させます。
            var sourcePos = gameObject.transform.position;
            StartCoroutine(MovePlayerProcess(sourcePos, targetWorldPos));
        }

        /// <summary>
        /// 操作キャラクターを移動させるコルーチンです。
        /// </summary>
        /// <param name="sourcePos">移動元の位置</param>
        /// <param name="targetPos">移動先の位置</param>
        IEnumerator MovePlayerProcess(Vector3 sourcePos, Vector3 targetPos)
        {
            // 完了後の時間を算出して、それまでの間、毎フレームLerpメソッドで位置を計算します。
            var animFinishTime = Time.time + _moveTime;
            var startedTime = Time.time;
            while (Time.time < animFinishTime)
            {
                var elapsedTime = Time.time - startedTime;
                var rate = Mathf.Clamp01(elapsedTime / _moveTime);
                Vector3 pos = Vector3.Lerp(sourcePos, targetPos, rate);
                gameObject.transform.position = pos;
                yield return null;
            }

            // 最終的な位置を設定して移動中フラグをfalseにします。
            gameObject.transform.position = targetPos;
            _isMoving = false;
        }
    }
}