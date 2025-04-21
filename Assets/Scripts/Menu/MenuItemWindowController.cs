using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メニュー画面のアイテムウィンドウを制御するクラスです。
    /// </summary>
    public class MenuItemWindowController : MonoBehaviour, IMenuWindowController, IMessageCallback
    {
        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照です。
        /// </summary>
        MenuManager _menuManager;

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuItemUIController _uiController;

        /// <summary>
        /// メニューウィンドウにてアイテムに関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuItemWindowItemController _itemController;

        /// <summary>
        /// 現在選択中の項目のインデックスです。
        /// </summary>
        int _selectedIndex;

        /// <summary>
        /// 現在のページ数です。
        /// </summary>
        int _page;

        /// <summary>
        /// 項目選択が可能かどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// メッセージをポーズするかどうかのフラグです。
        /// </summary>
        public bool IsPausedMessage { get; private set; }

        /// <summary>
        /// 1ページあたりのアイテム表示数です。
        /// </summary>
        readonly int ItemsInPage = 8;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        /// <summary>
        /// メニュー画面に関する機能を管理するクラスへの参照を取得します。
        /// </summary>
        public MenuManager GetMenuManager()
        {
            return _menuManager;
        }

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        void InitializeControllers()
        {
            _itemController.SetItemsInPage(ItemsInPage);
            _itemController.SetReferences(this);
        }

        void Update()
        {
            SelectItem();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_menuManager == null)
            {
                return;
            }

            if (_menuManager.MenuPhase != MenuPhase.Item
                && _menuManager.MenuPhase != MenuPhase.Magic)
            {
                return;
            }

            if (!_canSelect)
            {
                return;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SelectUpperItem();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SelectLowerItem();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                ShowNextPage();
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                ShowPrevPage();
            }
            else if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                OnPressedCancelButton();
            }
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        bool IsValidIndex(int index)
        {
            bool isValid = false;
            if (_menuManager.SelectedMenu == MenuCommand.Item)
            {
                isValid = _itemController.IsValidIndex(index);
            }
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        bool IsValidSelection()
        {
            bool isValid = false;
            if (_menuManager.SelectedMenu == MenuCommand.Item)
            {
                isValid = _itemController.IsValidSelection(_selectedIndex);
            }
            return isValid;
        }

        /// <summary>
        /// 次のページを表示します。
        /// </summary>
        void ShowNextPage()
        {
            // 最大ページ数が1以下の場合は何もしません。
            if (GetMaxPageNum() <= 1)
            {
                return;
            }

            _page++;

            // 次のページが存在しない場合は先頭のページに戻ります。
            if (_page >= GetMaxPageNum())
            {
                _page = 0;
            }

            // カーソルを先頭に移動します。
            _selectedIndex = _page * ItemsInPage;
            SetPageElement();

            PostSelection();
        }

        /// <summary>
        /// 前のページを表示します。
        /// </summary>
        void ShowPrevPage()
        {
            // 最大ページ数が1以下の場合は何もしません。
            if (GetMaxPageNum() <= 1)
            {
                return;
            }

            _page--;

            // 前のページが存在しない場合は最後尾のページに戻ります。
            if (_page < 0)
            {
                _page = GetMaxPageNum() - 1;
            }

            // カーソルを先頭に移動します。
            _selectedIndex = _page * ItemsInPage;
            SetPageElement();

            PostSelection();
        }

        /// <summary>
        /// ひとつ上の項目を選択します。
        /// </summary>
        void SelectUpperItem()
        {
            int newIndex = _selectedIndex - 1;
            int pageStartIndex = _page * ItemsInPage;
            if (newIndex < pageStartIndex)
            {
                int itemCount = _itemController.GetPageItemCount();
                newIndex = _page * ItemsInPage + itemCount - 1;
            }

            if (IsValidIndex(newIndex))
            {
                _selectedIndex = newIndex;
            }

            PostSelection();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            int newIndex = _selectedIndex + 1;
            int itemCount = _itemController.GetPageItemCount();
            int pageEndIndex = _page * ItemsInPage + itemCount - 1;
            if (newIndex > pageEndIndex)
            {
                newIndex = _page * ItemsInPage;
            }

            if (IsValidIndex(newIndex))
            {
                _selectedIndex = newIndex;
            }

            PostSelection();
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        int GetMaxPageNum()
        {
            int maxPage = 0;
            if (_menuManager.SelectedMenu == MenuCommand.Item)
            {
                maxPage = _itemController.GetMaxPageNum();
            }
            return maxPage;
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void PostSelection()
        {
            ShowSelectionCursor();
            ShowSelectedItemDescription();
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void ShowSelectionCursor()
        {
            int index = _selectedIndex % ItemsInPage;
            _uiController.ShowSelectedCursor(index);
        }

        /// <summary>
        /// 選択中の項目の説明を表示します。
        /// </summary>
        void ShowSelectedItemDescription()
        {
            if (_menuManager.SelectedMenu == MenuCommand.Item)
            {
                _itemController.SetItemDescription(_selectedIndex, _uiController);
            }
        }

        /// <summary>
        /// 現在のページ数テキストを表示します。
        /// </summary>
        void ShowPagerText()
        {
            int showPage = _page + 1;
            int maxPage = GetMaxPageNum();
            if (maxPage <= 0)
            {
                maxPage = 1;
            }
            string pageText = $"{showPage} / {maxPage}";
            _uiController.SetPagerText(pageText);
        }

        /// <summary>
        /// ウィンドウの状態をセットアップします。
        /// </summary>
        public void SetUpWindow()
        {
            _uiController.ClearAllItemText();
            _uiController.ClearDescriptionText();
            InitializeSelect();
            InitializeControllers();
        }

        /// <summary>
        /// 項目選択を初期化します。
        /// </summary>
        public void InitializeSelect()
        {
            _page = 0;
            _selectedIndex = 0;
            _uiController.ShowSelectedCursor(_selectedIndex);
            PostSelection();
        }

        /// <summary>
        /// ページ内の項目をセットします。
        /// </summary>
        public void SetPageElement()
        {
            if (_menuManager.SelectedMenu == MenuCommand.Item)
            {
                _itemController.SetPageItem(_page, _uiController);
            }

            // ページ送りのカーソルの表示状態を確認します。左右にループして表示するため、最大ページ数が1より大きい場合は表示するようにします。
            bool isVisiblePrevCursor = GetMaxPageNum() > 1;
            bool isVisibleNextCursor = GetMaxPageNum() > 1;
            _uiController.SetPrevCursorVisibility(isVisiblePrevCursor);
            _uiController.SetNextCursorVisibility(isVisibleNextCursor);
            ShowPagerText();
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            if (!IsValidSelection())
            {
                return;
            }

            if (_menuManager.SelectedMenu == MenuCommand.Item)
            {
                _itemController.UseSelectedItem(_selectedIndex);
            }
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        void OnPressedCancelButton()
        {
            StartCoroutine(HideProcess());
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        IEnumerator HideProcess()
        {
            SetCanSelectState(false);
            yield return null;

            _menuManager.OnItemCanceled();
            HideWindow();
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// 選択ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }

        /// <summary>
        /// 項目選択が可能かどうかの状態を設定します。
        /// </summary>
        /// <param name="canSelect">項目選択が可能かどうか</param>
        public void SetCanSelectState(bool canSelect)
        {
            _canSelect = canSelect;
        }

        /// <summary>
        /// メッセージの表示待ちを行うかどうかの状態を設定します。
        /// </summary>
        /// <param name="pause">メッセージをポーズするかどうか</param>
        public void SetPauseMessageState(bool pause)
        {
            IsPausedMessage = pause;
        }

        /// <summary>
        /// メッセージの表示が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            ShowNextMessage();
        }

        /// <summary>
        /// 次のメッセージを表示する処理です。
        /// </summary>
        void ShowNextMessage()
        {
            SetPauseMessageState(false);
        }

        /// <summary>
        /// アイテムや魔法の使用処理が終わった後の処理です。
        /// </summary>
        public void PostAction()
        {
            // 画面の更新を行います。
            SetPageElement();
            SetCanSelectState(true);
        }
    }
}