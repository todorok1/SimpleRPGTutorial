using UnityEngine;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// 選択ウィンドウを制御するクラスです。
    /// </summary>
    public class SelectionWindowController : MonoBehaviour
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
        /// 選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        SelectionUIController _uiController;

        /// <summary>
        /// 現在選択中の項目のインデックスです。
        /// </summary>
        int _selectedIndex;

        /// <summary>
        /// 現在のページ数です。
        /// </summary>
        int _page;

        /// <summary>
        /// 項目オブジェクトと魔法IDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _magicIdDictionary = new();

        /// <summary>
        /// 項目オブジェクトとアイテムIDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _itemIdDictionary = new();

        /// <summary>
        /// キャラクターが覚えている魔法の一覧です。
        /// </summary>
        List<MagicData> _characterMagicList = new();

        /// <summary>
        /// 項目選択が可能かどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(BattleManager battleManager, BattleUIManager uiManager)
        {
            _battleManager = battleManager;
            _uiManager = uiManager;
            _uiController = _uiManager.GetUIControllerSelectItem();
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
            if (_battleManager == null)
            {
                return;
            }

            if (_battleManager.BattlePhase != BattlePhase.SelectItem)
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
                SelectRightItem();
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                SelectLeftItem();
            }
            else if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                _battleManager.OnItemCanceled();
                SetCanSelectState(false);
            }
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        bool IsValidIndex(int index)
        {
            bool isValid = false;
            if (_battleManager.SelectedCommand == BattleCommand.Magic)
            {
                isValid = index >= 0 && index < _characterMagicList.Count;
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                isValid = index >= 0 && index < CharacterStatusManager.partyItemInfoList.Count;
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
            if (_battleManager.SelectedCommand == BattleCommand.Magic)
            {
                if (!_magicIdDictionary.ContainsKey(_selectedIndex))
                {
                    return isValid;
                }

                var magicId = _magicIdDictionary[_selectedIndex];
                var magicData = MagicDataManager.GetMagicDataById(magicId);
                var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
                var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
                isValid = characterStatus.currentMp >= magicData.cost;
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                if (!_itemIdDictionary.ContainsKey(_selectedIndex))
                {
                    return isValid;
                }

                var itemId = _itemIdDictionary[_selectedIndex];
                var partyItemInfo = CharacterStatusManager.partyItemInfoList.Find(info => info.itemId == itemId);
                isValid = partyItemInfo.itemNum > 0;
            }
            return isValid;
        }

        /// <summary>
        /// 右側の項目を選択します。
        /// </summary>
        void SelectRightItem()
        {
            int currentColmun = _selectedIndex % 2;
            int leftColumn = 0;

            // 左側の項目を選択していない場合は何もしません。
            if (currentColmun != leftColumn)
            {
                return;
            }

            // 移動先のインデックスに魔法が存在する場合はカーソルを移動します。
            if (IsValidIndex(_selectedIndex + 1))
            {
                _selectedIndex += 1;
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// 左側の項目を選択します。
        /// </summary>
        void SelectLeftItem()
        {
            int currentColmun = _selectedIndex % 2;
            int rightColumn = 1;

            // 右側の項目を選択していない場合は何もしません。
            if (currentColmun != rightColumn)
            {
                return;
            }

            // 移動先のインデックスに魔法が存在する場合はカーソルを移動します。
            if (IsValidIndex(_selectedIndex - 1))
            {
                _selectedIndex -= 1;
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// ひとつ上の項目を選択します。
        /// </summary>
        void SelectUpperItem()
        {
            int currentRow = _selectedIndex / 2;
            int upperRow = 0;
            int lowerRow = 1;
            if (currentRow == upperRow)
            {
                if (_page > 0)
                {
                    _page -= 1;
                    SetPageElement();
                    _selectedIndex = _page * 4;
                }
            }
            else if (currentRow == lowerRow)
            {
                if (IsValidIndex(_selectedIndex - 2))
                {
                    _selectedIndex -= 2;
                }
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            int currentRow = _selectedIndex / 2;
            int upperRow = 0;
            int lowerRow = 1;
            if (currentRow == lowerRow)
            {
                if (_page < GetMaxPageNum())
                {
                    _page += 1;
                    SetPageElement();
                }
            }
            else if (currentRow == upperRow)
            {
                if (IsValidIndex(_selectedIndex + 2))
                {
                    _selectedIndex += 2;
                }
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        int GetMaxPageNum()
        {
            int maxPage = 0;
            if (_battleManager.SelectedCommand == BattleCommand.Magic)
            {
                maxPage = Mathf.FloorToInt(_characterMagicList.Count / 4.0f);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                maxPage = Mathf.FloorToInt(CharacterStatusManager.partyItemInfoList.Count / 4.0f);
            }
            return maxPage;
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void ShowSelectionCursor()
        {
            int index = _selectedIndex % 4;
            _uiController.ShowSelectedCursor(index);
        }

        /// <summary>
        /// ウィンドウの状態をセットアップします。
        /// </summary>
        public void SetUpWindow()
        {
            SimpleLogger.Instance.Log($"SetUpWindow()が呼ばれました。");
            _uiController.SetUpControllerDictionary();
            _uiController.ClearAllItemText();
            _uiController.ClearDescriptionText();
            InitializeSelect();
            SetCharacterMagic();
        }

        /// <summary>
        /// 項目選択を初期化します。
        /// </summary>
        public void InitializeSelect()
        {
            _page = 0;
            _selectedIndex = SelectionItemPosition.LeftTop;
            _uiController.ShowSelectedCursor(_selectedIndex);
        }

        /// <summary>
        /// キャラクターが覚えている魔法をリストにセットします。
        /// </summary>
        void SetCharacterMagic()
        {
            _characterMagicList.Clear();

            // 指定したキャラクターのステータスを取得します。
            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            foreach (var magicId in characterStatus.magicList)
            {
                var magicData = MagicDataManager.GetMagicDataById(magicId);
                _characterMagicList.Add(magicData);
            }
        }

        /// <summary>
        /// ページ内の項目をセットします。
        /// </summary>
        public void SetPageElement()
        {
            _itemIdDictionary.Clear();
            _magicIdDictionary.Clear();
            if (_battleManager.SelectedCommand == BattleCommand.Magic)
            {
                SetPageMagic();
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                SetPageItem();
            }

            // ページ送りのカーソルの表示状態を確認します。
            bool isVisiblePrevCursor = _page > 0;
            bool isVisibleNextCursor = _page < GetMaxPageNum();
            _uiController.SetPrevCursorVisibility(isVisiblePrevCursor);
            _uiController.SetNextCursorVisibility(isVisibleNextCursor);
        }

        /// <summary>
        /// ページ内の魔法の項目をセットします。
        /// </summary>
        void SetPageMagic()
        {
            int startIndex = _page * 4;
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                int positionIndex = i - startIndex;
                if (i < _characterMagicList.Count)
                {
                    var magicData = _characterMagicList[i];
                    string magicName = magicData.magicName;
                    int magicCost = magicData.cost;
                    bool canSelect = CanSelectMagic(magicData);
                    _uiController.SetItemText(positionIndex, magicName, magicCost, canSelect);
                    _uiController.SetDescriptionText(magicData.magicDesc);
                    _magicIdDictionary.Add(positionIndex, magicData.magicId);
                }
                else
                {
                    _uiController.ClearItemText(positionIndex);
                }
            }

            if (_magicIdDictionary.Count == 0)
            {
                string noMagicText = "* 選択できる魔法がありません！ *";
                _uiController.SetDescriptionText(noMagicText);
            }
        }

        /// <summary>
        /// 魔法を使えるか確認します。
        /// </summary>
        bool CanSelectMagic(MagicData magicData)
        {
            var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
            return characterStatus.currentMp >= magicData.cost;
        }

        /// <summary>
        /// ページ内のアイテムの項目をセットします。
        /// </summary>
        void SetPageItem()
        {
            int startIndex = _page * 4;
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                int positionIndex = i - startIndex;
                if (i < CharacterStatusManager.partyItemInfoList.Count)
                {
                    var partyItemInfo = CharacterStatusManager.partyItemInfoList[i];
                    var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
                    string itemName = itemData.itemName;
                    int itemNum = partyItemInfo.itemNum;
                    bool canSelect = CanSelectItem(itemData);
                    _uiController.SetItemText(positionIndex, itemName, itemNum, canSelect);
                    _uiController.SetDescriptionText(itemData.itemDesc);
                    _itemIdDictionary.Add(positionIndex, itemData.itemId);
                }
                else
                {
                    _uiController.ClearItemText(positionIndex);
                }
            }

            if (_itemIdDictionary.Count == 0)
            {
                string noItemText = "* 選択できるアイテムがありません！ *";
                _uiController.SetDescriptionText(noItemText);
            }
        }

        /// <summary>
        /// アイテムを使えるか確認します。
        /// </summary>
        bool CanSelectItem(ItemData itemData)
        {
            var partyItemInfo = CharacterStatusManager.partyItemInfoList.Find(info => info.itemId == itemData.itemId);
            return partyItemInfo.itemNum > 0;
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

            if (_battleManager.SelectedCommand == BattleCommand.Magic)
            {
                if (_selectedIndex >= 0 && _selectedIndex < _characterMagicList.Count)
                {
                    var magicData = _characterMagicList[_selectedIndex];
                    _battleManager.OnItemSelected(magicData.magicId);
                    HideWindow();
                    SetCanSelectState(false);
                }
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                if (_selectedIndex >= 0 && _selectedIndex < CharacterStatusManager.partyItemInfoList.Count)
                {
                    var itemData = CharacterStatusManager.partyItemInfoList[_selectedIndex];
                    _battleManager.OnItemSelected(itemData.itemId);
                    HideWindow();
                    SetCanSelectState(false);
                }
            }
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
    }
}