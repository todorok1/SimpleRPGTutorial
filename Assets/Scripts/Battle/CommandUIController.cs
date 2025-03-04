using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// コマンドのUIを制御するクラスです。
    /// </summary>
    public class CommandUIController : BattleUIControllerBase
    {
        /// <summary>
        /// 攻撃コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjAttack;

        /// <summary>
        /// 魔法コマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjMagic;

        /// <summary>
        /// アイテムコマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjItem;

        /// <summary>
        /// 逃げるコマンドのカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjRun;

        /// <summary>
        /// コマンドのカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _cursorObjAttack.SetActive(false);
            _cursorObjMagic.SetActive(false);
            _cursorObjItem.SetActive(false);
            _cursorObjRun.SetActive(false);
        }

        /// <summary>
        /// 選択中のコマンドのカーソルを表示します。
        /// </summary>
        public void ShowSelectedCursor(BattleCommand command)
        {
            HideAllCursor();

            switch (command)
            {
                case BattleCommand.Attack:
                    _cursorObjAttack.SetActive(true);
                    break;
                case BattleCommand.Magic:
                    _cursorObjMagic.SetActive(true);
                    break;
                case BattleCommand.Item:
                    _cursorObjItem.SetActive(true);
                    break;
                case BattleCommand.Run:
                    _cursorObjRun.SetActive(true);
                    break;
            }
        }
    }
}