using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面全体を管理するクラスです。
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// メニューのトップ画面を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TopMenuWindowController _topMenuWindowController;

        /// <summary>
        /// メニューのフェーズです。
        /// </summary>
        public MenuPhase MenuPhase { get; private set; }

        /// <summary>
        /// 選択されたメニューです。
        /// </summary>
        public MenuCommand SelectedMenu { get; private set; }

        void Start()
        {
            
        }

        void Update()
        {
            CheckOpenMenuKey();
        }

        /// <summary>
        /// メニューを開くキーの入力を確認します。
        /// </summary>
        void CheckOpenMenuKey()
        {
            // 移動中以外の場合はメニューを開けないようにします。
            if (GameStateManager.CurrentState != GameState.Moving)
            {
                return;
            }

            // メニューが閉じている場合のみ、メニューを開くキーの入力を確認します。
            if (MenuPhase != MenuPhase.Closed)
            {
                return;
            }
            
            // メニューを開くキーが押された場合、メニューを開きます。
            if (InputGameKey.CancelButton())
            {
                OpenMenu();
            }
        }

        /// <summary>
        /// メニュー画面を表示します。
        /// </summary>
        void OpenMenu()
        {
            MenuPhase = MenuPhase.Top;
            _topMenuWindowController.SetUpController(this);
            _topMenuWindowController.InitializeCommand();
            _topMenuWindowController.ShowWindow();

            _characterMoverManager.StopCharacterMover();
        }

        /// <summary>
        /// メニュー項目が選択された時のコールバックです。
        /// </summary>
        public void OnSelectedMenu(MenuCommand menuCommand)
        {
            SelectedMenu = menuCommand;
            SimpleLogger.Instance.Log($"選択されたメニュー: {menuCommand}");
            HandleMenu();
        }

        /// <summary>
        /// 選択されたメニューに応じた処理を呼び出します。
        /// </summary>
        void HandleMenu()
        {
            switch (SelectedMenu)
            {
                case MenuCommand.Item:
                    // アイテムメニューを開く処理
                    break;
                case MenuCommand.Equipment:
                    // 装備メニューを開く処理
                    break;
                case MenuCommand.Status:
                    // ステータスメニューを開く処理
                    break;
                case MenuCommand.Save:
                    // セーブメニューを開く処理
                    break;
                case MenuCommand.QuitGame:
                    // ゲーム終了処理
                    break;
                case MenuCommand.Close:
                    _topMenuWindowController.CloseMenu();
                    break;
            }
        }

        /// <summary>
        /// メニュー画面が閉じる時のコールバックです。
        /// </summary>
        public void OnCloseMenu()
        {
            MenuPhase = MenuPhase.Closed;
            _characterMoverManager.ResumeCharacterMover();
        }
    }
}