using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターの移動を行うクラスの基底クラスです。
    /// </summary>
    public class CharacterMover : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動にかかる時間です。
        /// </summary>
        [SerializeField]
        protected float _moveTime = 0.25f;

        /// <summary>
        /// Tilemapを管理するクラスへの参照です。
        /// </summary>
        protected TilemapManager _tilemapManager;

        /// <summary>
        /// 移動するキャラクターのAnimatorです。
        /// </summary>
        protected Animator _animator;

        /// <summary>
        /// 移動するキャラクターのColliderです。
        /// </summary>
        protected BoxCollider2D _boxCollider2d;

        /// <summary>
        /// Tilemap上における現在の論理的な座標です。
        /// </summary>
        protected Vector3Int _posOnTile = Vector3Int.zero;

        /// <summary>
        /// 移動中かどうかのフラグです。
        /// </summary>
        protected bool _isMoving;

        protected void Start()
        {
            CheckComponents();
            GetCurrentPositionOnTilemap();
        }

        void Update()
        {
            
        }

        /// <summary>
        /// 必要なコンポーネントへの参照を確認します。
        /// </summary>
        protected virtual void CheckComponents()
        {
            if (_animator == null)
            {
                _animator = gameObject.GetComponent<Animator>();
            }

            if (_boxCollider2d == null)
            {
                _boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
            }

            if (_tilemapManager == null)
            {
                _tilemapManager = FindAnyObjectByType<TilemapManager>();
            }
        }

        /// <summary>
        /// Tilemap上の論理的な座標を取得します。
        /// </summary>
        protected void GetCurrentPositionOnTilemap()
        {
            if (_tilemapManager == null)
            {
                return;
            }

            _posOnTile = _tilemapManager.GetPositionOnTilemap(gameObject.transform.position);
        }

        /// <summary>
        /// キャラクターを移動させます。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="animDirection">アニメーションの方向</param>
        protected virtual void MoveCharacter(Vector2Int moveDirection, MoveAnimationDirection animDirection)
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

            // 移動先にキャラクターがいる場合は処理を抜けます。
            var sourcePos = gameObject.transform.position;
            bool existsCharacter = ExistsOtherCharacter(moveDirection, sourcePos);
            if (existsCharacter)
            {
                return;
            }

            // 論理的な座標から移動先の座標を計算し、ワールド座標に変換します。
            _posOnTile = targetPos;
            var targetWorldPos = _tilemapManager.GetWorldPosition(_posOnTile);

            // コルーチンを使ってキャラクターのゲームオブジェクトを移動させます。
            _isMoving = true;
            _tilemapManager.ReservePosition(GetInstanceID(), targetPos);
            StartCoroutine(MovePlayerProcess(sourcePos, targetWorldPos));
        }

        /// <summary>
        /// キャラクターを移動させるコルーチンです。
        /// </summary>
        /// <param name="sourcePos">移動元の位置</param>
        /// <param name="targetPos">移動先の位置</param>
        protected IEnumerator MovePlayerProcess(Vector3 sourcePos, Vector3 targetPos)
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

        /// <summary>
        /// 移動先に他のキャラクターがいるかどうか確認します。
        /// いる場合にTrueを返します。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="worldPos">移動元のワールド座標</param>
        protected virtual bool ExistsOtherCharacter(Vector2Int moveDirection, Vector3 worldPos)
        {
            if (_boxCollider2d == null)
            {
                return false;
            }

            // Rayを飛ばすにあたって、フィルター、結果を格納するリスト、距離を定義します。
            ContactFilter2D filter2D = new();
            List<RaycastHit2D> raycastHits = new();
            float distance = 1.0f;
            _boxCollider2d.Raycast(moveDirection, filter2D.NoFilter(), raycastHits, distance);

            // 返ってきた結果からColliderを取得してタグを確認します。
            bool exist = false;
            foreach (var hit in raycastHits)
            {
                var collider = hit.collider;
                if (collider == null)
                {
                    continue;
                }

                if (IsCollisionTargetTag(collider))
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }

        /// <summary>
        /// Colliderのタグが、移動しない対象のものか確認します。
        /// </summary>
        /// <param name="collider">RayがHitしたCollider</param>
        protected virtual bool IsCollisionTargetTag(Collider2D collider)
        {
            bool isTargetTag = collider.CompareTag(ObjectTagSettings.Npc)
                            || collider.CompareTag(ObjectTagSettings.Player);
            return isTargetTag;
        }
    }
}