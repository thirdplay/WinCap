using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

namespace WinCap.Driver.Controls
{
    /// <summary>
    /// ショートカットキーボックスに対応した操作を提供します。
    /// </summary>
    public class WPFShortcutKeyBox
    {
        /// <summary>
        /// アプリケーション内変数
        /// </summary>
        public AppVar AppVar { get; private set; }

        /// <summary>
        /// 現在のショートカットキーを取得します。
        /// </summary>
        public int[] Current
        {
            get { return (int[])this.AppVar["Current"]().Core; }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appVar">アプリケーション内変数</param>
        public WPFShortcutKeyBox(AppVar appVar)
        {
            this.AppVar = appVar;
        }

        /// <summary>
        /// 現在のショートカットキーを変更します。
        /// </summary>
        /// <param name="values">キーコード</param>
        public void EmulateChangeCurrent(int[] values)
        {
            AppVar.Dynamic().Focus();
            AppVar.Dynamic().Current = values;
        }
    }
}
