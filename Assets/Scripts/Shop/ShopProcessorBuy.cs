using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// お店でアイテムの購入処理を制御するクラスです。
    /// </summary>
    public class ShopProcessorBuy : MonoBehaviour
    {
        /// <summary>
        /// お店画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        ShopItemWindowController _windowController;

        /// <summary>
        /// メニューウィンドウにて購入に関する処理を制御するクラスへの参照です。
        /// </summary>
        ShopItemWindowBuyController _buyController;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(ShopItemWindowController windowController)
        {
            _windowController = windowController;
            _buyController = windowController.GetBuyController();
        }

        /// <summary>
        /// 引数のIDのアイテムを購入します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public void BuySelectedItem(int itemId)
        {
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {itemId}");
                return;
            }

            // アイテムの所持数を増やします。
            int itemNum = 1;
            CharacterStatusManager.IncreaseItem(itemData.itemId, itemNum);

            // アイテムの購入に伴うお金の減少を処理します。
            CharacterStatusManager.IncreaseGold(-itemData.price * itemNum);

            _buyController.OnFinishedItemProcess();
        }
    }
}