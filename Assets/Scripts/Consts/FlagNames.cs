namespace SimpleRpg
{
    /// <summary>
    /// フラグに関する定義を保持するクラスです。
    /// </summary>
    public static class FlagNames
    {
        /// <summary>
        /// オープニングイベントのフラグ名です。
        /// </summary>
        public const string Opening = "Opening";

        /// <summary>
        /// 洞窟の宝箱のフラグ名です。
        /// </summary>
        public const string DungeonTreasure = "DungeonTreasure";

        /// <summary>
        /// 洞窟のボス討伐のフラグ名です。
        /// </summary>
        public const string DungeonBossWin = "DungeonBossWin";

        /// <summary>
        /// 戦闘に負けた時のフラグ名です。
        /// </summary>
        public const string BattleLose = "BattleLose";
    }
}