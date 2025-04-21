using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <summary>
    /// メニューウィンドウにてアイテムに関する処理を制御するクラスです。
    /// </summary>
    public class MenuItemWindowItemController : MonoBehaviour
    {
        /// <summary>
        /// メニュー画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        MenuItemWindowController _windowController;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        MapMessageWindowController _mapMessageWindowController;

        /// <summary>
        /// 項目オブジェクトとアイテムIDの対応辞書です。
        /// </summary>
        Dictionary<int, int> _itemIdDictionary = new();

        /// <summary>
        /// 1ページあたりのアイテム表示数です。
        /// </summary>
        int _itemInPage;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(MenuItemWindowController windowController)
        {
            _windowController = windowController;
            _mapMessageWindowController = windowController.GetMenuManager().GetMessageWindowController();
        }

        /// <summary>
        /// ページ内に表示するアイテム数をセットします。
        /// </summary>
        /// <param name="itemNum">ページ内のアイテム数</param>
        public void SetItemsInPage(int itemNum)
        {
            _itemInPage = itemNum;
        }

        /// <summary>
        /// ページ内に存在するアイテム数を取得します。
        /// </summary>
        public int GetPageItemCount()
        {
            return _itemIdDictionary.Count;
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        public bool IsValidIndex(int index)
        {
            bool isValid = index >= 0 && index < CharacterStatusManager.partyItemInfoList.Count;
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        /// <param name="selectedIndex">選択中のインデックス</param>
        public bool IsValidSelection(int selectedIndex)
        {
            bool isValid = false;
            int indexInPage = selectedIndex % _itemInPage;
            if (!_itemIdDictionary.ContainsKey(indexInPage))
            {
                return isValid;
            }

            var itemId = _itemIdDictionary[indexInPage];
            var partyItemInfo = CharacterStatusManager.partyItemInfoList.Find(info => info.itemId == itemId);
            isValid = partyItemInfo.itemNum > 0;
            return isValid;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        public int GetMaxPageNum()
        {
            int maxPage = Mathf.CeilToInt(CharacterStatusManager.partyItemInfoList.Count * 1.0f / _itemInPage * 1.0f);
            return maxPage;
        }

        /// <summary>
        /// ページ内のアイテムの項目をセットします。
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetPageItem(int page, MenuItemUIController uiController)
        {
            _itemIdDictionary.Clear();
            int startIndex = page * _itemInPage;
            for (int i = startIndex; i < startIndex + _itemInPage; i++)
            {
                int positionIndex = i - startIndex;
                if (i < CharacterStatusManager.partyItemInfoList.Count)
                {
                    var partyItemInfo = CharacterStatusManager.partyItemInfoList[i];
                    var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
                    string itemName = itemData.itemName;
                    int itemNum = partyItemInfo.itemNum;
                    bool canSelect = CanSelectItem(partyItemInfo);
                    uiController.SetItemText(positionIndex, itemName, itemNum, canSelect);
                    _itemIdDictionary.Add(positionIndex, itemData.itemId);
                }
                else
                {
                    uiController.ClearItemText(positionIndex);
                }
            }

            if (_itemIdDictionary.Count == 0)
            {
                uiController.ClearDescriptionText();
            }
        }

        /// <summary>
        /// 引数のインデックスに対応するアイテムの説明をセットします。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        /// <param name="uiController">UIの制御クラス</param>
        public void SetItemDescription(int selectedIndex, MenuItemUIController uiController)
        {
            if (!IsValidIndex(selectedIndex))
            {
                return;
            }

            var partyItemInfo = GetItemInfo(selectedIndex);
            if (partyItemInfo == null)
            {
                return;
            }

            int itemId = partyItemInfo.itemId;
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData != null)
            {
                uiController.SetDescriptionText(itemData.itemDesc);
            }
        }

        /// <summary>
        /// アイテムを使えるか確認します。
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        bool CanSelectItem(PartyItemInfo partyItemInfo)
        {
            if (partyItemInfo == null)
            {
                return false;
            }

            var itemData = ItemDataManager.GetItemDataById(partyItemInfo.itemId);
            if (itemData == null)
            {
                return false;
            }

            if (itemData.itemCategory != ItemCategory.ConsumableItem)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 選択されたインデックスに対応するアイテム情報を取得します。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public PartyItemInfo GetItemInfo(int selectedIndex)
        {
            PartyItemInfo itemInfo = null;
            if (selectedIndex >= 0 && selectedIndex < CharacterStatusManager.partyItemInfoList.Count)
            {
                itemInfo = CharacterStatusManager.partyItemInfoList[selectedIndex];
            }
            return itemInfo;
        }

        /// <summary>
        /// 選択されたインデックスに対応するアイテムを使用します。
        /// </summary>
        /// <param name="selectedIndex">選択されたインデックス</param>
        public void UseSelectedItem(int selectedIndex)
        {
            PartyItemInfo itemInfo = GetItemInfo(selectedIndex);
            if (!CanSelectItem(itemInfo))
            {
                return;
            }

            var itemData = ItemDataManager.GetItemDataById(itemInfo.itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {itemInfo.itemId}");
                return;
            }

            // 消費アイテムの場合、所持数を減らします。
            if (itemData.itemCategory == ItemCategory.ConsumableItem)
            {
                CharacterStatusManager.UseItem(itemData.itemId);
            }

            int targetCharacterId = CharacterStatusManager.partyCharacter[0];
            if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.Recovery)
            {
                int hpDelta = BattleCalculator.CalculateHealValue(itemData.itemEffect.value);
                int mpDelta = 0;
                CharacterStatusManager.ChangeCharacterStatus(targetCharacterId, hpDelta, mpDelta);
                StartCoroutine(ShowItemHealMessage(targetCharacterId, itemData.itemName, hpDelta));
            }
        }

        /// <summary>
        /// 回復アイテムのメッセージを表示します。
        /// </summary>
        IEnumerator ShowItemHealMessage(int characterId, string itemName, int healValue)
        {
            var characterName = CharacterDataManager.GetCharacterName(characterId);
            string actorName = characterName;
            string targetName = characterName;

            _mapMessageWindowController.SetUpController(_windowController);
            _mapMessageWindowController.HidePager();
            _mapMessageWindowController.ShowWindow();

            _windowController.SetCanSelectState(false);
            _windowController.SetPauseMessageState(true);
            _mapMessageWindowController.GenerateUseItemMessage(actorName, itemName);
            while (_windowController.IsPausedMessage)
            {
                yield return null;
            }

            _windowController.SetPauseMessageState(true);
            _mapMessageWindowController.GenerateHpHealMessage(targetName, healValue);
            while (_windowController.IsPausedMessage)
            {
                yield return null;
            }

            // キー入力を待ちます。
            _mapMessageWindowController.StartKeyWait();
            while (_mapMessageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }

            _windowController.PostAction();
            _mapMessageWindowController.HideWindow();
        }
    }
}