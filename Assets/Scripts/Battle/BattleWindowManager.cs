using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘関連のウィンドウ全体を管理するクラスです。
    /// </summary>
    public class BattleWindowManager : MonoBehaviour
    {
        /// <summary>
        /// ステータス表示のウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        StatusWindowController _statusWindowController;

        /// <summary>
        /// 敵キャラクターの名前を表示するウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyNameWindowController _enemyNameWindowController;

        /// <summary>
        /// コマンドウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CommandWindowController _commandWindowController;

        /// <summary>
        /// 選択ウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionWindowController _selectItemWindowControllerSelect;

        /// <summary>
        /// メッセージウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MessageWindowController _messageWindowController;

        /// <summary>
        /// 各ウィンドウのコントローラをセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        /// <param name="uIManager">戦闘関連のUI全体を管理するクラス</param>
        public void SetUpWindowControllers(BattleManager battleManager, BattleUIManager uIManager)
        {
            _statusWindowController.SetUpController(uIManager);
            _enemyNameWindowController.SetUpController(uIManager);
            _commandWindowController.SetUpController(battleManager, uIManager);
            _selectItemWindowControllerSelect.SetUpController(battleManager, uIManager);
            _messageWindowController.SetUpController(battleManager, uIManager);
        }

        /// <summary>
        /// ステータス表示のウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public StatusWindowController GetStatusWindowController()
        {
            return _statusWindowController;
        }

        /// <summary>
        /// 敵キャラクターの名前を表示するウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public EnemyNameWindowController GetUIControllerEnemyName()
        {
            return _enemyNameWindowController;
        }

        /// <summary>
        /// コマンドウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public CommandWindowController GetCommandWindowController()
        {
            return _commandWindowController;
        }

        /// <summary>
        /// 選択ウィンドウを制御するクラスへの参照を取得します。
        /// </summary>
        public SelectionWindowController GetSelectionWindowController()
        {
            return _selectItemWindowControllerSelect;
        }

        /// <summary>
        /// メッセージウィンドウの動作を制御するクラスへの参照を取得します。
        /// </summary>
        public MessageWindowController GetMessageWindowController()
        {
            return _messageWindowController;
        }
    }
}