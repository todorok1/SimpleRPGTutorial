namespace SimpleRpg
{
    /// <summary>
    /// メニューに関する状態を定義する列挙型です。
    /// </summary>
    public enum MenuPhase
    {
        Closed,
        Top,
        Item,
        Equipment,
        Status,
        Save,
        QuitGame,
    }
}