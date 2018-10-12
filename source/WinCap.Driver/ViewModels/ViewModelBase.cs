using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using System.Collections.Generic;

namespace WinCap.Driver.ViewModels
{
    /// <summary>
    /// ViewModelの基底クラス。
    /// </summary>
    public abstract class ViewModelBase
    {
        /// <summary>
        /// アプリケーション内変数
        /// </summary>
        private AppVar appVar;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appVar">アプリケーション内変数</param>
        public ViewModelBase(AppVar appVar)
        {
            this.appVar = appVar;
            RevertToSavedSettings();
        }

        /// <summary>
        /// 保存時の設定に戻します。
        /// </summary>
        public void RevertToSavedSettings()
        {
            this.appVar.Dynamic().RevertToSavedSettings();
        }

        /// <summary>
        /// プロパティ名のエラーメッセージを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public string GetError(string propertyName)
        {
            var errors = (List<string>)this.appVar.Dynamic().GetErrors(propertyName);
            if (errors?.Count > 0)
            {
                return errors[0];
            }
            return string.Empty;
        }
    }
}
