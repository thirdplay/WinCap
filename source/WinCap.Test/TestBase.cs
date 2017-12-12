using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using WinCap.Driver;
using WinCap.Driver.Drivers;

namespace WinCap.Test
{
    /// <summary>
    /// テストの基底クラスです。
    /// </summary>
    public class TestBase<T>
    {
        /// <summary>
        /// アプリケーションドライバーを取得または設定します。
        /// </summary>
        protected static AppDriver App { get; set; }

        /// <summary>
        /// テスト結果。
        /// </summary>
        private static Dictionary<string, bool> tests;

        /// <summary>
        /// テストコンテキストを取得、設定します。
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// テストクラスを初期化します。
        /// </summary>
        public static void NotifyClassInitialize()
        {
            App = new AppDriver();
            tests = typeof(T).GetMethods().Where(e => 0 < e.GetCustomAttributes(typeof(TestMethodAttribute), true).Length).ToDictionary(e => e.Name, e => true);
        }

        /// <summary>
        /// テストクラスを終了化します。
        /// </summary>
        public static void NotifyClassCleanup()
        {
            App.EndProcess();
        }

        /// <summary>
        /// テスト開始処理。
        /// </summary>
        public void NotifyTestInitialize()
        {
            App.Attach();
        }

        /// <summary>
        /// テスト終了処理。
        /// </summary>
        public void NotifyTestCleanup()
        {
            if (this.TestContext.DataRow == null ||
                ReferenceEquals(this.TestContext.DataRow, this.TestContext.DataRow.Table.Rows[this.TestContext.DataRow.Table.Rows.Count - 1]))
            {
                tests.Remove(this.TestContext.TestName);
            }
            App.Release(this.TestContext.CurrentTestOutcome == UnitTestOutcome.Passed && 0 < tests.Count);
        }

        /// <summary>
        /// パラメータを取得します。
        /// </summary>
        /// <typeparam name="Data">パラメータを表すクラス</typeparam>
        /// <returns>パラメータのインスタンス</returns>
        public Data GetParam<Data>() where Data : new()
        {
            Data data = new Data();
            foreach (var e in typeof(Data).GetProperties())
            {
                e.GetSetMethod().Invoke(data, new object[] { Convert(e.PropertyType, this.TestContext.DataRow[e.Name]) });
            }
            return data;
        }

        /// <summary>
        /// <see cref="obj"/> を<see cref="type"/> に変換して返却します。
        /// </summary>
        /// <param name="type">変換する型</param>
        /// <param name="obj">変換対象の値</param>
        /// <returns>返還後の値</returns>
        private static object Convert(Type type, object obj)
        {
            // 必要に応じて変換方法を追加
            string value = obj == null ? string.Empty : obj.ToString();
            if (type == typeof(int))
            {
                return int.Parse(value);
            }
            else if (type == typeof(bool))
            {
                return string.Compare(value, true.ToString(), true) == 0;
            }
            else if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(int[]))
            {
                return (string.IsNullOrEmpty(value))
                    ? Array.Empty<int>()
                    : value.Split(',').Select(x => int.Parse(x)).ToArray();
            }
            else if (type == typeof(string[]))
            {
                return value.Split(',').Select(x => x.Trim()).ToArray();
            }
            throw new NotSupportedException();
        }
    }
}
