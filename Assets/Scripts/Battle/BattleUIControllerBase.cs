using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘関連のUIを制御するクラスの基底クラスです。
    /// </summary>
    public class BattleUIControllerBase : MonoBehaviour
    {
        /// <summary>
        /// 戦闘のUIを表示します。
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 戦闘のUIを非表示にします。
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}