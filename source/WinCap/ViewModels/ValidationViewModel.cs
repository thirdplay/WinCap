using Livet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WinCap.ViewModels
{
    /// <summary>
    /// バリデーション機能を提供するViewModelの基底クラスです。
    /// </summary>
    public abstract class ValidationViewModel : ViewModel, INotifyDataErrorInfo
    {
        /// <summary>
        /// 各プロパティのエラーコンテナ
        /// </summary>
        private readonly Dictionary<string, List<string>> currentErrors = new Dictionary<string, List<string>>();

        #region INotifyDataErrorInfoの実装
        /// <summary>
        /// 検証エラーイベント
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 検証エラー変更イベントを発生させる。
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定したプロパティまたはエンティティ全体の検証エラーを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>検証エラー</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (this.HasErrors)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    var allErrors = new List<string>();
                    foreach (var errors in this.currentErrors.Values)
                    {
                        allErrors.AddRange(errors);
                    }
                    return allErrors;
                }
                if (this.currentErrors.ContainsKey(propertyName))
                {
                    return this.currentErrors[propertyName];
                }
            }
            return null;
        }

        /// <summary>
        /// 検証エラーがあるかどうか取得します。
        /// </summary>
        public bool HasErrors => this.currentErrors.Count > 0;
        #endregion

        /// <summary>
        /// プロパティの入力値を検証する
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>検証エラーがある場合はtrue、それ以外はfalse</returns>
        protected bool validate([CallerMemberName]string propertyName = null)
        {
            object value = this.GetType().GetProperty(propertyName).GetValue(this);
            var context = new ValidationContext(this) { MemberName = propertyName };
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                var errors = validationErrors.Select(error => error.ErrorMessage);
                setErrors(propertyName, errors);
            }
            else
            {
                clearErrors(propertyName);
            }

            return this.HasErrors;
        }

        /// <summary>
        /// 全てのプロパティの入力値を検証する
        /// </summary>
        /// <returns>検証エラーがある場合はtrue、それ以外はfalse</returns>
        protected bool validateAll()
        {
            clearErrorsAll();
            var context = new ValidationContext(this);
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(this, context, validationErrors, true))
            {
                var errors = validationErrors.Where(x => x.MemberNames.Any()).GroupBy(x => x.MemberNames.First());
                foreach (var error in errors)
                {
                    setErrors(error.Key, error.Select(x => x.ErrorMessage));
                }
            }

            return this.HasErrors;
        }

        /// <summary>
        /// 引数で指定されたプロパティに、<see cref="errors"/> で指定されたエラーをすべて登録します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="errors">エラーリスト</param>
        protected void setErrors(string propertyName, IEnumerable<string> errors)
        {
            var hasCurrentError = this.currentErrors.ContainsKey(propertyName);
            var hasNewError = errors != null && errors.Count() > 0;

            if (!hasCurrentError && !hasNewError)
            {
                return;
            }

            if (hasNewError)
            {
                this.currentErrors[propertyName] = new List<string>(errors);
            }
            else
            {
                this.currentErrors.Remove(propertyName);
            }
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// 引数で指定されたプロパティのエラーをすべて解除します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected void clearErrors(string propertyName)
        {
            if (this.currentErrors.ContainsKey(propertyName))
            {
                this.currentErrors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// エンティティ全体のエラーをすべて解除します。
        /// </summary>
        protected void clearErrorsAll()
        {
            while (currentErrors.Count > 0)
            {
                string key = currentErrors.First().Key;
                this.currentErrors.Remove(key);
                OnErrorsChanged(key);
            }
        }
    }
}
