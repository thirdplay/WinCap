using System.Collections.Generic;
using System.Collections.Specialized;

namespace WinCap.Util.Collections.Generic
{
    /// <summary>
    /// キーと値のペアのインデックス付きジェネリック コレクションを表します。
    /// </summary>
    /// <typeparam name="TKey">ディクショナリ内のキーの型</typeparam>
    /// <typeparam name="TValue">ディクショナリ内の値の型</typeparam>
    public interface IOrderedDictionary<TKey, TValue> : IOrderedDictionary, IDictionary<TKey, TValue>
    {
        /// <summary>
        /// 指定したインデックス位置にある要素を取得または設定します。
        /// </summary>
        /// <param name="index">取得または設定する要素の、0 から始まるインデックス番号</param>
        /// <returns>インデックス位置にある要素</returns>
        new TValue this[int index] { get; set; }

        /// <summary>
        /// コレクション内の指定したインデックス位置にキーと値のペアを挿入します。
        /// </summary>
        /// <param name="index">キーと値のペアを挿入する位置の0から始まるインデックス</param>
        /// <param name="key">追加する要素のキー</param>
        /// <param name="value">追加する要素の値</param>
        void Insert(int index, TKey key, TValue value);
    }
}
