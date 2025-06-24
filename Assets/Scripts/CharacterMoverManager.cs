using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターの移動を行うクラスを管理するクラスです。
    /// </summary>
    public class CharacterMoverManager : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動を行うクラスのリストです。
        /// </summary>
        List<CharacterMover> _characterMovers = new();

        /// <summary>
        /// キャラクターの移動を行うクラスを登録します。
        /// </summary>
        /// <param name="characterMover">キャラクターの移動を行うクラス</param>
        public void RegisterCharacterMover(CharacterMover characterMover)
        {
            _characterMovers.Add(characterMover);
        }

        /// <summary>
        /// キャラクターの移動とアニメーションを停止します。
        /// </summary>
        public void StopCharacterMover()
        {
            SimpleLogger.Instance.Log("StopCharacterMover()が呼ばれました。");
            foreach (var characterMover in _characterMovers)
            {
                if (characterMover == null)
                {
                    continue;
                }
                characterMover.StopAnimation();
                characterMover.StopMoving();
            }
        }

        /// <summary>
        /// キャラクターの移動とアニメーションを再開します。
        /// </summary>
        public void ResumeCharacterMover()
        {
            SimpleLogger.Instance.Log("ResumeCharacterMover()が呼ばれました。");
            foreach (var characterMover in _characterMovers)
            {
                if (characterMover == null)
                {
                    continue;
                }
                characterMover.ResumeAnimation();
                characterMover.ResumeMoving();
            }
        }

        /// <summary>
        /// キャラクターのタイル上の位置をリセットします。
        /// </summary>
        public void ResetPositions()
        {
            foreach (var characterMover in _characterMovers)
            {
                if (characterMover == null)
                {
                    continue;
                }
                characterMover.ResetPosition();
            }
        }
    }
}