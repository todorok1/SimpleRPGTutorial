using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// コマンドウィンドウを制御するクラスです。
    /// </summary>
    public class CommandWindowController : MonoBehaviour
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘関連のUI全体を管理するクラスへの参照です。
        /// </summary>
        BattleUIManager _uiManager;

        /// <summary>
        /// コマンドウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        BattleUIControllerCommand _uiController;

        /// <summary>
        /// 現在選択中のコマンドです。
        /// </summary>
        BattleCommand _selectedCommand;

        /// <summary>
        /// コマンドを選択できるかどうかのフラグです。
        /// </summary>
        bool _canSelectCommand;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(BattleManager battleManager, BattleUIManager uiManager)
        {
            _battleManager = battleManager;
            _uiManager = uiManager;
            _uiController = _uiManager.GetUIControllerCommand();
        }

        void Update()
        {
            SelectCommand();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectCommand()
        {
            if (_battleManager == null)
            {
                return;
            }

            if (_battleManager.BattlePhase != BattlePhase.InputCommand)
            {
                return;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SetPreCommand();
                _uiController.ShowSelectedCursor(_selectedCommand);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SetNextCommand();
                _uiController.ShowSelectedCursor(_selectedCommand);
            }
            else if (InputGameKey.ConfirmButton())
            {
                _battleManager.OnCommandSelected(_selectedCommand);
            }
        }

        /// <summary>
        /// ひとつ前のコマンドを選択します。
        /// </summary>
        void SetPreCommand()
        {
            int currentCommand = (int)_selectedCommand;
            int nextCommand = currentCommand - 1;
            if (nextCommand < 0)
            {
                nextCommand = 3;
            }
            _selectedCommand = (BattleCommand)nextCommand;
        }

        /// <summary>
        /// ひとつ後のコマンドを選択します。
        /// </summary>
        void SetNextCommand()
        {
            int currentCommand = (int)_selectedCommand;
            int nextCommand = currentCommand + 1;
            if (nextCommand > 3)
            {
                nextCommand = 0;
            }
            _selectedCommand = (BattleCommand)nextCommand;
        }

        /// <summary>
        /// コマンド選択を初期化します。
        /// </summary>
        public void InitializeCommand()
        {
            _selectedCommand = BattleCommand.Attack;
            _uiController.ShowSelectedCursor(_selectedCommand);
        }

        /// <summary>
        /// コマンドウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// コマンドウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}