using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のメニューを管理するクラスです。
    /// </summary>
    public class TitleMenuManager : MonoBehaviour
    {
        /// <summary>
        /// タイトルメニューのトップ画面を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        TitleMenuWindowController _titleMenuWindowController;

        /// <summary>
        /// 選択されたメニューです。
        /// </summary>
        public TitleCommand SelectedMenu { get; private set; }

        void Start()
        {
            InitializeMenu();
        }

        /// <summary>
        /// メニューを初期化します。
        /// </summary>
        public void InitializeMenu()
        {
            _titleMenuWindowController.InitializeCommand();
        }

        /// <summary>
        /// メニューを選択できるようにします。
        /// </summary>
        public void StartSelect()
        {
            _titleMenuWindowController.SetUpController(this);
            _titleMenuWindowController.SetCanSelectState(true);
        }

        /// <summary>
        /// メニュー項目が選択された時のコールバックです。
        /// </summary>
        public void OnSelectedMenu(TitleCommand titleCommand)
        {
            SelectedMenu = titleCommand;
            SimpleLogger.Instance.Log($"選択されたメニュー: {titleCommand}");
            HandleMenu();
        }

        /// <summary>
        /// 選択されたメニューに応じた処理を呼び出します。
        /// </summary>
        void HandleMenu()
        {
            switch (SelectedMenu)
            {
                case TitleCommand.Start:
                    // はじめからゲームを開始します。
                    break;
                case TitleCommand.Continue:
                    // ロード画面を表示します。
                    break;
                case TitleCommand.Quit:
                   // ゲームを終了します。
                    break;
            }
        }

        /// <summary>
        /// ロード画面でキャンセルされた時のコールバックです。
        /// </summary>
        public void OnLoadCanceled()
        {
            InitializeMenu();
            StartSelect();
        }

        /// <summary>
        /// ロード画面でセーブ枠が選択された時のコールバックです。
        /// </summary>
        public void OnSelectedSlotId(int slotId)
        {
            
        }
    }
}