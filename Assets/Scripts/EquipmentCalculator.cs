namespace SimpleRpg
{
    /// <summary>
    /// 装備品による補正を計算するクラスです。
    /// </summary>
    public static class EquipmentCalculator
    {
        /// <summary>
        /// 装備品のIDから補正値を取得します。
        /// </summary>
        public static CorrectedValue GetCorrectedParameter(int weaponId, int armorId)
        {
            CorrectedValue correctedValue = new();
            CalculateCorrectedValue(weaponId, correctedValue);
            CalculateCorrectedValue(armorId, correctedValue);
            return correctedValue;
        }

        /// <summary>
        /// IDからアイテムデータを取得します。
        /// </summary>
        static void CalculateCorrectedValue(int itemId, CorrectedValue correctedValue)
        {
            ItemData itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData != null)
            {
                correctedValue.strength += itemData.strength;
                correctedValue.guard += itemData.guard;
                correctedValue.speed += itemData.speed;
            }
        }
    }
}