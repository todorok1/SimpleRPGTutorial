using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// お店でアイテムの売却処理を制御するクラスです。
    /// </summary>
    public class ShopProcessorSell : MonoBehaviour
    {
        /// <summary>
        /// お店画面のアイテムウィンドウを制御するクラスへの参照です。
        /// </summary>
        ShopItemWindowController _windowController;

        /// <summary>
        /// メニューウィンドウにて売却に関する処理を制御するクラスへの参照です。
        /// </summary>
        ShopItemWindowSellController _sellController;

        /// <summary>
        /// 参照をセットアップします。
        /// </summary>
        /// <param name="windowController">メニューウィンドウを制御するクラス</param>
        public void SetReferences(ShopItemWindowController windowController)
        {
            _windowController = windowController;
            _sellController = windowController.GetSellController();
        }

        /// <summary>
        /// 引数のIDのアイテムを売却します。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        public void SellSelectedItem(int itemId)
        {
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {itemId}");
                return;
            }

            // アイテムの所持数を減らします。
            int itemNum = 1;
            CharacterStatusManager.DecreaseItem(itemData.itemId, itemNum);

            // アイテムの売却に伴うお金の減少を処理します。
            int price = (int) (itemData.price * ValueSettings.SellPriceMultiplier);
            CharacterStatusManager.IncreaseGold(price * itemNum);

            _sellController.OnFinishedItemProcess();
        }
    }
}