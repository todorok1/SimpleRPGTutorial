using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘関連のUI全体を管理するクラスです。
    /// </summary>
    public class BattleUIManager : MonoBehaviour
    {
        /// <summary>
        /// ステータス表示のUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        BattleUIControllerStatus _battleUIControllerStatus;

        /// <summary>
        /// 敵キャラクターの名前表示のUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        BattleUIControllerEnemyName _battleUIControllerEnemyName;

        /// <summary>
        /// コマンドのUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        BattleUIControllerCommand _battleUIControllerCommand;

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        BattleUIControllerSelectItem _battleUIControllerSelectItem;

        /// <summary>
        /// メッセージウィンドウのUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        BattleUIControllerMessage _battleUIControllerMessage;

        /// <summary>
        /// UIコントローラのリストです。
        /// </summary>
        List<BattleUIControllerBase> _battleUIControllers = new();

        void Start()
        {
            SetControllerList();
        }

        void Update()
        {
            
        }

        /// <summary>
        /// UIコントローラのリストをセットアップします。
        /// </summary>
        public void SetControllerList()
        {
            _battleUIControllers = new()
            {
                _battleUIControllerStatus,
                _battleUIControllerEnemyName,
                _battleUIControllerCommand,
                _battleUIControllerSelectItem,
                _battleUIControllerMessage
            };
        }

        /// <summary>
        /// 各UIを非表示にします。
        /// </summary>
        public void HideAllUI()
        {
            foreach (var controller in _battleUIControllers)
            {
                controller.Hide();
            }
        }

        /// <summary>
        /// 指定したUIを表示します。
        /// </summary>
        /// <param name="targetUIController">表示対象のUI</param>
        public void ShowTargetUI(BattleUIControllerBase targetUIController)
        {
            targetUIController.Show();
        }

        /// <summary>
        /// 指定したUIを非表示にします。
        /// </summary>
        /// <param name="targetUIController">非表示対象のUI</param>
        public void HideTargetUI(BattleUIControllerBase targetUIController)
        {
            targetUIController.Hide();
        }

        /// <summary>
        /// ステータス表示のUIを取得します。
        /// </summary>
        public BattleUIControllerStatus GetUIControllerStatus()
        {
            return _battleUIControllerStatus;
        }

        /// <summary>
        /// 敵キャラクターの名前表示のUIを取得します。
        /// </summary>
        public BattleUIControllerEnemyName GetUIControllerEnemyName()
        {
            return _battleUIControllerEnemyName;
        }

        /// <summary>
        /// コマンド表示のUIを取得します。
        /// </summary>
        public BattleUIControllerCommand GetUIControllerCommand()
        {
            return _battleUIControllerCommand;
        }

        /// <summary>
        /// 選択ウィンドウのUIを取得します。
        /// </summary>
        public BattleUIControllerSelectItem GetUIControllerSelectItem()
        {
            return _battleUIControllerSelectItem;
        }

        /// <summary>
        /// 選択ウィンドウのUIを取得します。
        /// </summary>
        public BattleUIControllerMessage GetUIControllerMessage()
        {
            return _battleUIControllerMessage;
        }
    }
}