using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// メッセージウィンドウの動作を制御するクラスです。
    /// </summary>
    public class MessageWindowController : MonoBehaviour
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
        /// メッセージウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        BattleUIControllerMessage _uiController;

        /// <summary>
        /// メッセージの表示間隔です。
        /// </summary>
        [SerializeField]
        float _messageInterval = 1.0f;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(BattleManager battleManager, BattleUIManager uiManager)
        {
            _battleManager = battleManager;
            _uiManager = uiManager;
            _uiController = _uiManager.GetUIControllerMessage();
        }

        /// <summary>
        /// ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.ClearMessage();
            _uiController.Show();
        }

        /// <summary>
        /// ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.ClearMessage();
            _uiController.Hide();
        }

        /// <summary>
        /// 攻撃時のメッセージを生成します。
        /// </summary>
        public void GenerateEnemyAppearMessage(string enemyName, float appearInterval)
        {
            _uiController.ClearMessage();
            string message = $"{enemyName}{BattleMessage.EnemyAppearSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message, appearInterval));
        }

        /// <summary>
        /// 攻撃時のメッセージを生成します。
        /// </summary>
        public void GenerateAttackMessage(string attackerName)
        {
            _uiController.ClearMessage();
            string message = $"{attackerName}{BattleMessage.AttackSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// ダメージ発生時のメッセージを生成します。
        /// </summary>
        public void GenerateDamageMessage(string targetName, int damage)
        {
            string message = $"{targetName}{BattleMessage.DefendSuffix} {damage} {BattleMessage.DamageSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 敵を撃破した時のメッセージを生成します。
        /// </summary>
        public void GenerateDefeateEnemyMessage(string targetName)
        {
            string message = $"{targetName}{BattleMessage.DefeatEnemySuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 味方がやられた時のメッセージを生成します。
        /// </summary>
        public void GenerateDefeateFriendMessage(string targetName)
        {
            string message = $"{targetName}{BattleMessage.DefeatFriendSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 魔法を唱えた時のメッセージを生成します。
        /// </summary>
        public void GenerateMagicCastMessage(string magicUserName, string magicName)
        {
            _uiController.ClearMessage();
            string message = $"{magicUserName}{BattleMessage.MagicUserSuffix} {magicName} {BattleMessage.MagicNameSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// HPが回復する時のメッセージを生成します。
        /// </summary>
        public void GenerateHpHealMessage(string targetName, int healNum)
        {
            string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// アイテムを使用した時のメッセージを生成します。
        /// </summary>
        public void GenerateUseItemMessage(string itemUserName, string itemName)
        {
            _uiController.ClearMessage();
            string message = $"{itemUserName}{BattleMessage.ItemUserSuffix} {itemName} {BattleMessage.ItemNameSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 逃走した時のメッセージを生成します。
        /// </summary>
        public void GenerateRunMessage(string characterName)
        {
            _uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.RunnerSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 逃走が失敗した時のメッセージを生成します。
        /// </summary>
        public void GenerateRunFailedMessage()
        {
            string message = BattleMessage.RunFailed;
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// ゲームオーバー時のメッセージを生成します。
        /// </summary>
        public void GenerateGameoverMessage(string characterName)
        {
            _uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.GameoverSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// 戦闘勝利時のメッセージを生成します。
        /// </summary>
        public void GenerateWinMessage(string characterName)
        {
            _uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.WinSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// Exp獲得時のメッセージを生成します。
        /// </summary>
        public void GenerateGetExpMessage(int exp)
        {
            string message = $"{exp} {BattleMessage.GetExpSuffixSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// ゴールド獲得時のメッセージを生成します。
        /// </summary>
        public void GenerateGetGoldMessage(int gold)
        {
            string message = $"{gold} {BattleMessage.GetGoldSuffixSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// レベルアップ時のメッセージを生成します。
        /// </summary>
        public void GenerateLevelUpMessage(string characterName, int level)
        {
            _uiController.ClearMessage();
            string message = $"{characterName}{BattleMessage.LevelUpNameSuffix} {level} {BattleMessage.LevelUpNumberSuffix}";
            StartCoroutine(ShowMessageAutoProcess(message));
        }

        /// <summary>
        /// ページ送りのカーソルを表示します。
        /// </summary>
        public void ShowPager()
        {
            _uiController.ShowCursor();
        }

        /// <summary>
        /// ページ送りのカーソルを非表示にします。
        /// </summary>
        public void HidePager()
        {
            _uiController.HideCursor();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        IEnumerator ShowMessageAutoProcess(string message)
        {
            _uiController.AppendMessage(message);
            yield return new WaitForSeconds(_messageInterval);
            _battleManager.OnFinishedShowMessage();
        }

        /// <summary>
        /// メッセージを順番に表示するコルーチンです。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="interval">表示間隔</param>
        IEnumerator ShowMessageAutoProcess(string message, float interval)
        {
            SimpleLogger.Instance.Log(message);
            _uiController.AppendMessage(message);
            yield return new WaitForSeconds(interval);
            _battleManager.OnFinishedShowMessage();
        }
    }
}