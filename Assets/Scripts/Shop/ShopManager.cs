using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleRpg
{
    /// <summary>
    /// お店画面全体を管理するクラスです。
    /// </summary>
    public class ShopManager : MonoBehaviour, IMessageCallback, IOptionCallback, IShopOptionCallback
    {
        /// <summary>
        /// お店の選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        ShopOptionWindowController _shopOptionWindowController;

        /// <summary>
        /// 通常の選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        OptionWindowController _optionWindowController;

        /// <summary>
        /// マップ上で表示するメッセージウィンドウを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MapMessageWindowController _mapMessageWindowController;

        /// <summary>
        /// お店の処理が完了した際に呼び出されるコールバックです。
        /// </summary>
        IShopCallback _callback;

        /// <summary>
        /// 購入または売却対象のアイテムデータです。
        /// </summary>
        ItemData _selectedItemData;

        /// <summary>
        /// お店のトップ画面を表示するのが2回目以降かどうかのフラグです。
        /// </summary>
        bool _isSecondVisit = false;

        /// <summary>
        /// お店の店主の名前です。
        /// </summary>
        string _shopMasterName = string.Empty;

        /// <summary>
        /// お店に表示するアイテムのIDリストです。
        /// </summary>
        List<int> _shopItems = new();

        /// <summary>
        /// お店に表示するアイテムのIDリストです。
        /// </summary>
        public List<int> ShopItems
        {
            get { return _shopItems; }
        }

        /// <summary>
        /// 選択されたお店のメニューです。
        /// </summary>
        ShopCommand _selectedCommand = ShopCommand.None;

        /// <summary>
        /// 選択されたお店のメニューです。
        /// </summary>
        public ShopCommand SelectedCommand
        {
            get { return _selectedCommand; }
        }

        /// <summary>
        /// お店に入った時のメッセージです。
        /// </summary>
        readonly string WelcomeMessage = "いらっしゃい！\n必要なものはあるかい？";

        /// <summary>
        /// 2回目以降のメッセージです。
        /// </summary>
        readonly string SecondMessage = "他に必要なものはあるかい？";

        /// <summary>
        /// デフォルトの店主の名前です。
        /// </summary>
        readonly string DefaultShopMasterName = "店主";

        /// <summary>
        /// お店の選択肢ウィンドウの動作を制御するクラスへの参照です。
        /// </summary>
        /// <param name="callback">お店の処理が完了した際に呼び出されるコールバック</param>
        /// <param name="shopItemIds">お店に表示するアイテムのIDリスト</param>
        /// <param name="shopMasterName">お店の店主の名前</param>
        public void StartShopProcess(IShopCallback callback, List<int> shopItemIds, string shopMasterName)
        {
            _callback = callback;
            _shopItems = shopItemIds;
            _shopMasterName = shopMasterName;

            _isSecondVisit = false;
            _selectedCommand = ShopCommand.None;
            ShowWelcomeMessage();
        }

        /// <summary>
        /// お店に入った時のメッセージを表示します。
        /// </summary>
        public void ShowWelcomeMessage()
        {
            if (_mapMessageWindowController == null)
            {
                SimpleLogger.Instance.LogError("MapMessageWindowControllerがアサインされていません。");
                return;
            }

            string message = _isSecondVisit ? SecondMessage : WelcomeMessage;
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.SetUpController(this);
            _mapMessageWindowController.ShowWindow();
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, 0.5f);
        }

        /// <summary>
        /// タグを含めた店主の名前を取得します。
        /// </summary>
        string GetMasterName()
        {
            string masterName = _shopMasterName;
            if (string.IsNullOrEmpty(_shopMasterName))
            {
                // 店主の名前が設定されていない場合はデフォルトの名前を使用します。
                masterName = DefaultShopMasterName;
            }
            return $"<{masterName}>";
        }

        /// <summary>
        /// 店主の名前を含めた表示用のメッセージを取得します。
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        string GetFullMessage(string message)
        {
            string masterName = GetMasterName();
            string fullMessage = $"{masterName}\n{message}";
            return fullMessage;
        }

        /// <summary>
        /// メッセージの表示が完了したことを通知するコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            switch (_selectedCommand)
            {
                case ShopCommand.None:
                    ShowShopOption();
                    break;
            }
        }

        /// <summary>
        /// お店の選択肢ウィンドウを表示します。
        /// </summary>
        public void ShowShopOption()
        {
            if (_shopOptionWindowController == null)
            {
                SimpleLogger.Instance.LogError("ShopOptionWindowControllerがアサインされていません。");
                return;
            }

            _shopOptionWindowController.RegisterShopCallback(this);
            _shopOptionWindowController.SetUpController(this);
            _shopOptionWindowController.ShowWindow();
        }

        /// <summary>
        /// 通常の選択肢ウィンドウを表示します。
        /// </summary>
        public void ShowOption()
        {
            if (_optionWindowController == null)
            {
                SimpleLogger.Instance.LogError("OptionWindowControllerがアサインされていません。");
                return;
            }

            _optionWindowController.SetUpController(this);
            _optionWindowController.ShowWindow();
        }

        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        public void OnSelectedOption(int selectedIndex)
        {
            _mapMessageWindowController.HideWindow();
            SimpleLogger.Instance.Log($"OnSelectedOption()で選択されたインデックス: {selectedIndex}");
        }

        /// <summary>
        /// 選択肢が選ばれたことを通知するコールバックです。
        /// </summary>
        /// <param name="selectedIndex">選択された選択肢のインデックス</param>
        public void OnSelectedShopOption(int selectedIndex)
        {
            _mapMessageWindowController.HideWindow();
            if (selectedIndex == 0)
            {
                // 買うを選択した場合の処理です。
                _selectedCommand = ShopCommand.Buy;
                _mapMessageWindowController.HideWindow();
                SimpleLogger.Instance.Log("「買う」を選択しました。");
            }
            else if (selectedIndex == 1)
            {
                // 売るを選択した場合の処理です。
                _selectedCommand = ShopCommand.Sell;
                _mapMessageWindowController.HideWindow();
                SimpleLogger.Instance.Log("「売る」を選択しました。");
            }
            else
            {
                // キャンセルを選択した場合はイベントに戻ります。
                _selectedCommand = ShopCommand.Exit;
                ShowExitMessage();
            }
        }

        /// <summary>
        /// 退店時のメッセージを表示します。
        /// </summary>
        void ShowExitMessage()
        {
            StartCoroutine(WaitExitMessage());
        }

        /// <summary>
        /// 退店時のメッセージを表示します。
        /// </summary>
        IEnumerator WaitExitMessage()
        {
            yield return null;
            string message = $"またおいで！";
            string fullMessage = GetFullMessage(message);
            _mapMessageWindowController.ShowWindow();
            _mapMessageWindowController.ShowPager();
            _mapMessageWindowController.ShowGeneralMessage(fullMessage, 0.5f);
            _mapMessageWindowController.StartKeyWait();
            while (_mapMessageWindowController.IsWaitingKeyInput)
            {
                yield return null;
            }
            _mapMessageWindowController.HidePager();
            _mapMessageWindowController.HideWindow();
            ExitProcess();
        }

        /// <summary>
        /// 退店時の処理です。
        /// </summary>
        void ExitProcess()
        {
            // お店の終了を通知します。
            if (_callback != null)
            {
                _callback.OnFinishedShopProcess();
            }
        }

        /// <summary>
        /// 購入画面または売却画面がキャンセルされた時の処理です。
        /// </summary>
        public void OnCanceled()
        {
            _selectedCommand = ShopCommand.None;
            _isSecondVisit = true;
            ShowWelcomeMessage();
        }
    }
}