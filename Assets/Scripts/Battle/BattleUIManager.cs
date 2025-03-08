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
        StatusUIController _statusUIController;

        /// <summary>
        /// 敵キャラクターの名前表示のUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        EnemyNameUIController _enemyNameUIController;

        /// <summary>
        /// コマンドのUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        CommandUIController _commandUIController;

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        SelectionUIController _selectItemUIControllerSelect;

        /// <summary>
        /// メッセージウィンドウのUIを制御するクラスです。
        /// </summary>
        [SerializeField]
        MessageUIController _messageUIController;

        /// <summary>
        /// UIコントローラのリストです。
        /// </summary>
        List<IBattleUIController> _battleUIControllers = new();

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
                _statusUIController,
                _enemyNameUIController,
                _commandUIController,
                _selectItemUIControllerSelect,
                _messageUIController
            };
        }

        /// <summary>
        /// 各UIを非表示にします。
        /// </summary>
        public void HideAllWindow()
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
        /// ステータス表示のUIを制御するクラスへの参照を取得します。
        /// </summary>
        public StatusUIController GetUIControllerStatus()
        {
            return _statusUIController;
        }

        /// <summary>
        /// 敵キャラクターの名前表示のUIを制御するクラスへの参照を取得します。
        /// </summary>
        public EnemyNameUIController GetUIControllerEnemyName()
        {
            return _enemyNameUIController;
        }

        /// <summary>
        /// コマンド表示のUIを制御するクラスへの参照を取得します。
        /// </summary>
        public CommandUIController GetUIControllerCommand()
        {
            return _commandUIController;
        }

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスへの参照を取得します。
        /// </summary>
        public SelectionUIController GetUIControllerSelectItem()
        {
            return _selectItemUIControllerSelect;
        }

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスへの参照を取得します。
        /// </summary>
        public MessageUIController GetUIControllerMessage()
        {
            return _messageUIController;
        }
    }
}