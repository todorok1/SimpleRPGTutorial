using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘の開始処理を行うクラスです。
    /// </summary>
    public class BattleStarter : MonoBehaviour
    {
        /// <summary>
        /// スプライトの表示を制御するクラスへの参照です。
        /// </summary>
        BattleSpriteController _battleSpriteController;

        /// <summary>
        /// 戦闘開始メッセージを表示する時間です。
        /// </summary>
        [SerializeField]
        float _startMessageTime = 1.5f;

        /// <summary>
        /// 戦闘の管理を行うクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle(BattleManager battleManager)
        {
            _battleManager = battleManager;
            SetBattleState();
            SetBattlePhase();

            // 戦闘関連のUIを非表示にします。
            HideAllUI();

            // スプライトを表示します。
            ShowSprites();

            // ステータスのUIを表示します。
            ShowStatus();

            // コマンドウィンドウを表示します。
            ShowCommand();

            // 敵の名前ウィンドウを表示します。
            ShowEnemyNameWindow();

            // 敵出現のメッセージを表示します。
            ShowEnemyMessage();
        }

        /// <summary>
        /// ゲーム全体の状態を戦闘に切り替えます。
        /// </summary>
        void SetBattleState()
        {
            SetBattlePhase();
            GameStateManager.ChangeToBattle();
        }

        /// <summary>
        /// 戦闘のフェーズをエンカウントに切り替えます。
        /// </summary>
        void SetBattlePhase()
        {
            _battleManager.SetBattlePhase(BattlePhase.ShowEnemy);
        }

        /// <summary>
        /// 戦闘関連のUIを全て非表示にします。
        /// </summary>
        void HideAllUI()
        {
            _battleManager.GetUIManager().HideAllUI();
        }

        /// <summary>
        /// 戦闘関連のスプライトを表示します。
        /// </summary>
        void ShowSprites()
        {
            if (_battleSpriteController == null)
            {
                _battleSpriteController = _battleManager.GetBattleSpriteController();
            }

            _battleSpriteController.SetSpritePosition();
            _battleSpriteController.ShowBackground();
            _battleSpriteController.ShowEnemy(_battleManager.EnemyId);
        }

        /// <summary>
        /// 現在のステータスを表示します。
        /// </summary>
        void ShowStatus()
        {
            int characterId = 1;
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                Debug.LogWarning($"キャラクターステータスが取得できませんでした。 ID : {characterId}");
                return;
            }

            var controller = _battleManager.GetUIManager().GetUIControllerStatus();
            controller.SetCharacterStatus(characterStatus);
            controller.Show();
        }

        /// <summary>
        /// コマンド入力のUIを表示します。
        /// </summary>
        void ShowCommand()
        {
            var controller = _battleManager.GetCommandWindowController();
            controller.ShowWindow();
            controller.InitializeCommand();
        }

        /// <summary>
        /// 敵キャラクターの名前表示ウィンドウを表示します。
        /// </summary>
        void ShowEnemyNameWindow()
        {
            var controller = _battleManager.GetUIManager().GetUIControllerEnemyName();
            controller.Show();

            int enemyId = _battleManager.EnemyId;
            var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
            controller.SetEnemyName(enemyData.enemyName);
        }

        /// <summary>
        /// 敵キャラクターが出現したメッセージを表示します。
        /// </summary>
        void ShowEnemyMessage()
        {
            int enemyId = _battleManager.EnemyId;
            var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
            if (enemyData == null)
            {
                Debug.LogWarning($"敵データが取得できませんでした。 ID : {enemyId}");
                return;
            }

            // メッセージ表示後、BattleManagerに制御が戻ります。
            var controller = _battleManager.GetMessageWindowController();
            controller.ShowWindow();
            controller.GenerateEnemyAppearMessage(enemyData.enemyName, _startMessageTime);
        }
    }
}