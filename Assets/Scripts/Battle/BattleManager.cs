using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する機能を管理するクラスです。
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        /// <summary>
        /// 戦闘開始の処理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleStarter _battleStarter;

        /// <summary>
        /// 戦闘関連のウィンドウ全体を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleWindowManager _battleWindowManager;

        /// <summary>
        /// 戦闘関連のスプライトを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleSpriteController _battleSpriteController;

        /// <summary>
        /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        CharacterMoverManager _characterMoverManager;

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 敵キャラクターのコマンドを選択するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyCommandSelector _enemyCommandSelector;

        /// <summary>
        /// 戦闘中のアクションを登録するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionRegister _battleActionRegister;

        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessor _battleActionProcessor;

        /// <summary>
        /// 戦闘のフェーズです。
        /// </summary>
        public BattlePhase BattlePhase { get; private set; }

        /// <summary>
        /// 選択されたコマンドです。
        /// </summary>
        public BattleCommand SelectedCommand { get; private set; }

        /// <summary>
        /// エンカウントした敵キャラクターのIDです。
        /// </summary>
        public int EnemyId { get; private set; }

        /// <summary>
        /// 戦闘開始からのターン数です。
        /// </summary>
        public int TurnCount { get; private set; }

        /// <summary>
        /// 戦闘が終了したかどうかのフラグです。
        /// </summary>
        public bool IsBattleFinished { get; private set; }

        /// <summary>
        /// 戦闘のフェーズを変更します。
        /// </summary>
        /// <param name="battlePhase">変更後のフェーズ</param>
        public void SetBattlePhase(BattlePhase battlePhase)
        {
            BattlePhase = battlePhase;
        }

        /// <summary>
        /// 敵キャラクターのステータスをセットします。
        /// </summary>
        /// <param name="enemyId">敵キャラクターのID</param>
        public void SetUpEnemyStatus(int enemyId)
        {
            EnemyId = enemyId;
            _enemyStatusManager.SetUpEnemyStatus(enemyId);
        }

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle()
        {
            SimpleLogger.Instance.Log("戦闘を開始します。");
            GameStateManager.ChangeToBattle();
            SetBattlePhase(BattlePhase.ShowEnemy);

            _battleWindowManager.SetUpWindowControllers(this);
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HidePager();

            _battleActionProcessor.InitializeProcessor(this);
            _battleActionRegister.InitializeRegister(_battleActionProcessor);
            _enemyCommandSelector.SetReferences(this, _battleActionRegister);
            _characterMoverManager.StopCharacterMover();
            _battleStarter.StartBattle(this);
        }

        /// <summary>
        /// ウィンドウの管理を行うクラスへの参照を取得します。
        /// </summary>
        public BattleWindowManager GetWindowManager()
        {
            return _battleWindowManager;
        }

        /// <summary>
        /// 戦闘関連のスプライトを制御するクラスへの参照を取得します。
        /// </summary>
        public BattleSpriteController GetBattleSpriteController()
        {
            return _battleSpriteController;
        }

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照を取得します。
        /// </summary>
        public EnemyStatusManager GetEnemyStatusManager()
        {
            return _enemyStatusManager;
        }

        /// <summary>
        /// コマンド入力を開始します。
        /// </summary>
        public void StartInputCommandPhase()
        {
            SimpleLogger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HideWindow();
            BattlePhase = BattlePhase.InputCommand;
            _battleActionProcessor.InitializeActions();
        }

        /// <summary>
        /// コマンドが選択された時のコールバックです。
        /// </summary>
        public void OnCommandSelected(BattleCommand selectedCommand)
        {
            SimpleLogger.Instance.Log($"コマンドが選択されました: {selectedCommand}");
            SelectedCommand = selectedCommand;
            HandleCommand();
        }

        /// <summary>
        /// コマンド入力に応じた処理を行います。
        /// </summary>
        void HandleCommand()
        {
            SimpleLogger.Instance.Log($"入力されたコマンドに応じた処理を行います。選択されたコマンド: {SelectedCommand}");
            switch (SelectedCommand)
            {
                case BattleCommand.Attack:
                case BattleCommand.Run:
                    BattlePhase = BattlePhase.Action;
                    break;
                case BattleCommand.Magic:
                case BattleCommand.Item:
                    ShowSelectionWindow();
                    break;
            }
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        void ShowSelectionWindow()
        {
            SimpleLogger.Instance.Log($"ShowSelectionWindow()が呼ばれました。選択されたコマンド: {SelectedCommand}");
            StartCoroutine(ShowSelectionWindowProcess());
        }

         /// <summary>
        /// 選択ウィンドウを表示する処理です。
        /// </summary>
        IEnumerator ShowSelectionWindowProcess()
        {
            yield return null;
            BattlePhase = BattlePhase.SelectItem;
            var selectionWindowController = _battleWindowManager.GetSelectionWindowController();
            selectionWindowController.SetUpWindow();
            selectionWindowController.SetPageElement();
            selectionWindowController.ShowWindow();
            selectionWindowController.SetCanSelectState(true);
        }

        /// <summary>
        /// 選択ウィンドウで項目が選択された時のコールバックです。
        /// </summary>
        public void OnItemSelected(int itemId)
        {
            switch (SelectedCommand)
            {
                case BattleCommand.Magic:
                    SimpleLogger.Instance.Log($"選択された魔法のID: {itemId}");
                    break;
                case BattleCommand.Item:
                    SimpleLogger.Instance.Log($"選択されたアイテムのID: {itemId}");
                    break;
            }
        }

        /// <summary>
        /// 選択ウィンドウでキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnItemCanceled()
        {
            BattlePhase = BattlePhase.InputCommand;
        }

        /// <summary>
        /// メッセージウィンドウでメッセージの表示が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            switch (BattlePhase)
            {
                case BattlePhase.ShowEnemy:
                    SimpleLogger.Instance.Log("敵の表示が完了しました。");
                    StartInputCommandPhase();
                    break;
                case BattlePhase.Action:
                    _battleActionProcessor.ShowNextMessage();
                    break;
            }
        }

        /// <summary>
        /// ターン内の行動が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedActions()
        {
            if (IsBattleFinished)
            {
                SimpleLogger.Instance.Log("OnFinishedActions() || 戦闘が終了しているため、処理を中断します。");
                return;
            }

            SimpleLogger.Instance.Log("ターン内の行動が完了しました。");
            TurnCount++;
            StartInputCommandPhase();
        }

        /// <summary>
        /// コマンド選択が完了した後の処理です。
        /// </summary>
        void PostCommandSelect()
        {
            SimpleLogger.Instance.Log("敵のコマンド入力を行います。");
            _enemyCommandSelector.SelectEnemyCommand();
        }

        /// <summary>
        /// 敵キャラクターのコマンドが選択された時のコールバックです。
        /// </summary>
        public void OnEnemyCommandSelected()
        {
            SimpleLogger.Instance.Log("敵味方の行動が決まったので実際に行動させます。");
        }

        /// <summary>
        /// ステータスの値が更新された時のコールバックです。
        /// </summary>
        public void OnUpdateStatus()
        {
            _battleWindowManager.GetStatusWindowController().UpdateAllCharacterStatus();
        }

        /// <summary>
        /// 敵を全て倒した時のコールバックです。
        /// </summary>
        public void OnEnemyDefeated()
        {
            SimpleLogger.Instance.Log("敵を全て倒しました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
        }

        /// <summary>
        /// ゲームオーバーになった時のコールバックです。
        /// </summary>
        public void OnGameover()
        {
            SimpleLogger.Instance.Log("ゲームオーバーになりました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
        }

        /// <summary>
        /// 味方が逃走に成功した時のコールバックです。
        /// </summary>
        public void OnRunaway()
        {
            SimpleLogger.Instance.Log("逃走に成功しました。");
            IsBattleFinished = true;
            OnFinishBattle();
        }

        /// <summary>
        /// 敵が逃走に成功した時のコールバックです。
        /// </summary>
        public void OnEnemyRunaway()
        {
            SimpleLogger.Instance.Log("敵が逃走に成功しました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
        }

        /// <summary>
        /// 戦闘を終了する時のコールバックです。
        /// </summary>
        public void OnFinishBattle()
        {
            SimpleLogger.Instance.Log("戦闘に勝利して終了します。");
            BattlePhase = BattlePhase.NotInBattle;
        }
    }
}