using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスです。
    /// </summary>
    public class EnemyNameWindowController : MonoBehaviour
    {
        /// <summary>
        /// 戦闘関連のUI全体を管理するクラスへの参照です。
        /// </summary>
        BattleUIManager _uiManager;

        /// <summary>
        /// 敵キャラクターの名前を表示するUIを制御するクラスへの参照です。
        /// </summary>
        EnemyNameUIController _uiController;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="uiManager">戦闘関連のUI全体を管理するクラス</param>
        public void SetUpController(BattleUIManager uiManager)
        {
            _uiManager = uiManager;
            _uiController = _uiManager.GetUIControllerEnemyName();
        }

        /// <summary>
        /// 敵キャラクターの名前をセットします。
        /// </summary>
        /// <param name="enemyName">敵キャラクターの名前</param>
        public void SetEnemyName(string enemyName)
        {
            _uiController.SetEnemyName(enemyName);
        }

        /// <summary>
        /// 敵キャラクターの名前を空欄にします。
        /// </summary>
        public void ClearEnemyName()
        {
            _uiController.ClearEnemyName();
        }
    }
}