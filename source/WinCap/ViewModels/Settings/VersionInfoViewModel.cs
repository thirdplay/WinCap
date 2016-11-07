using System;
using System.ComponentModel.DataAnnotations;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// バージョン情報のためのデータを提供します。
    /// </summary>
    public class VersionInfoViewModel : SettingsBaseViewModel
    {
        #region TabItemViewModel mebmers
        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.Settings_VersionInfo; }
            protected set { throw new NotImplementedException(); }
        }
        #endregion

        #region SettingsBaseViewModel members
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
        }

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public override bool Validate()
        {
            return this.ValidateAll();
        }

        /// <summary>
        /// 適用
        /// </summary>
        public override void Apply()
        {
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public override void Cancel()
        {
        }
        #endregion
    }
}