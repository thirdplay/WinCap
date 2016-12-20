using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace WinCap.Util.Collections.Generic
{
    /// <summary>
    /// キーまたはインデックスからアクセスできる、順序付けられたキーと値のコレクションを表します。
    /// </summary>
    /// <typeparam name="TKey">ディクショナリ内のキーの型。</typeparam>
    /// <typeparam name="TValue">ディクショナリ内の値の型。</typeparam>
    [Serializable]
    [ComVisible(false)]
    public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>,
        IDictionary, ISerializable, IDeserializationCallback
    {
        #region Fields

        private Dictionary<TKey, TValue> _dic;
        private List<TKey> _list;
        private int _initialCapacity;
        private IEqualityComparer<TKey> _eqComparer;
        private KeyCollection _keys;
        private ValueCollection _values;
        private SerializationInfo _si;
        private object _syncRoot;
        private int _version;

        private const string VersionName = "Version";
        private const string EqualityComparerName = "EqualityComparer";
        private const string InitialCapacityName = "InitialCapacity";
        private const string KeyValuePairsName = "KeyValuePairs";

        #endregion

        #region Constructors

        /// <summary>
        /// 空で、既定の初期量を備え、キーの型の既定の等値比較子を使用する、 <see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public OrderedDictionary()
            : this(0, null)
        {
        }

        /// <summary>
        /// 指定した <see cref="IDictionary{TKey, TValue}"/> から要素をコピーして格納し、キーの型の既定の等値比較子を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="dictionary">新しい <see cref="OrderedDictionary{TKey, TValue}"/> に要素がコピーされる <see cref="IDictionary{TKey, TValue}"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        /// <summary>
        /// 指定した <see cref="IDictionary{TKey, TValue}"/> から要素をコピーして格納し、指定した <see cref="IEqualityComparer{T}"/> を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="dictionary">新しい <see cref="OrderedDictionary{TKey, TValue}"/> に要素がコピーされる <see cref="IDictionary{TKey, TValue}"/></param>
        /// <param name="comparer">キーの比較時に使用する <see cref="IEqualityComparer{T}"/> 実装。キーの型の既定の <see cref="EqualityComparer{T}"/> を使用する場合は null。</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            ThrowIf(dictionary == null, () => new ArgumentNullException(nameof(dictionary)));

            _initialCapacity = dictionary.Count;
            _eqComparer = comparer ?? EqualityComparer<TKey>.Default;

            _dic = new Dictionary<TKey, TValue>(_initialCapacity, _eqComparer);
            _list = new List<TKey>();

            foreach (var e in dictionary)
                Add(e.Key, e.Value);
        }

        /// <summary>
        /// 空で、既定の初期量を備え、指定した <see cref="IEqualityComparer{T}"/> を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparer">キーの比較時に使用する <see cref="IEqualityComparer{T}"/> 実装。キーの型の既定の <see cref="EqualityComparer{T}"/> を使用する場合は null。</param>
        public OrderedDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        /// <summary>
        /// 空で、指定した初期量を備え、キーの型の既定の等値比較子を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="capacity"><see cref="OrderedDictionary{TKey, TValue}"/> が格納できる要素数の初期値。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> が 0 未満です。</exception>
        public OrderedDictionary(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// 空で、指定した初期量を備え、指定した <see cref="IEqualityComparer{T}"/> を使用する、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="capacity"><see cref="OrderedDictionary{TKey, TValue}"/> が格納できる要素数の初期値。</param>
        /// <param name="comparer">キーの比較時に使用する <see cref="IEqualityComparer{T}"/> 実装。キーの型の既定の <see cref="EqualityComparer{T}"/> を使用する場合は null。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> が 0 未満です。</exception>
        public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            ThrowIf(capacity < 0, () => new ArgumentOutOfRangeException(nameof(capacity)));

            _initialCapacity = capacity;
            _eqComparer = comparer ?? EqualityComparer<TKey>.Default;

            _dic = new Dictionary<TKey, TValue>(_initialCapacity, _eqComparer);
            _list = new List<TKey>(_initialCapacity);
        }

        /// <summary>
        /// シリアル化したデータを使用して、<see cref="OrderedDictionary{TKey, TValue}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="info"><see cref="OrderedDictionary{TKey, TValue}"/> をシリアル化するために必要な情報を格納している <see cref="SerializationInfo"/> オブジェクト。</param>
        /// <param name="context"><see cref="OrderedDictionary{TKey, TValue}"/> に関連付けられているシリアル化ストリームの送信元および送信先を格納している <see cref="StreamingContext"/> 構造体。</param>
        protected OrderedDictionary(SerializationInfo info, StreamingContext context)
        {
            _si = info;
        }

        #endregion

        #region Properties

        /// <summary>
        /// ディクショナリのキーが等しいかどうかを確認するために使用する <see cref="IEqualityComparer{T}"/> を取得します。
        /// </summary>
        /// <value>現在の <see cref="OrderedDictionary{TKey, TValue}"/> のキーが等しいかどうかを確認し、キーのハッシュ値を提供するために使用する <see cref="IEqualityComparer{T}"/> ジェネリック インターフェイスの実装。</value>
        public IEqualityComparer<TKey> Comparer => _eqComparer ?? EqualityComparer<TKey>.Default;

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> に格納されているキー/値ペアの数を取得します。
        /// </summary>
        /// <value><see cref="OrderedDictionary{TKey, TValue}"/> に格納されているキー/値ペアの数。</value>
        public int Count => _list.Count;

        /// <summary>
        /// 指定したインデックスにある要素を取得または設定します。
        /// </summary>
        /// <param name="index">取得または設定する要素の、0 から始まるインデックス番号。</param>
        /// <returns>指定したインデックス位置にある要素。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> 以上になっています。</exception>
        public TValue this[int index]
        {
            get
            {
                ThrowIf(index < 0 || _list.Count <= index, () => new ArgumentOutOfRangeException(nameof(index)));

                return _dic[_list[index]];
            }
            set
            {
                ThrowIf(index < 0 || _list.Count <= index, () => new ArgumentOutOfRangeException(nameof(index)));

                _dic[_list[index]] = value;
                _version++;
            }
        }

        /// <summary>
        /// 指定されたキーに関連付けられている値を取得または設定します。
        /// </summary>
        /// <param name="key">取得または設定する値のキー。</param>
        /// <returns>指定されたキーに関連付けられている値。 指定したキーが見つからなかった場合、get 操作は <see cref="KeyNotFoundException"/> をスローし、set 操作は指定したキーを持つ新しい要素を作成します。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> が null です。</exception>
        /// <exception cref="KeyNotFoundException">プロパティが取得されましたが、コレクション内に <paramref name="key"/> が存在しません。</exception>
        public TValue this[TKey key]
        {
            get
            {
                ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

                if (!_dic.ContainsKey(key))
                {
                    throw new KeyNotFoundException(nameof(key));
                }

                return _dic[key];
            }
            set
            {
                ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

                if (_dic.ContainsKey(key))
                {
                    _dic[key] = value;
                    var idx = _list.FindIndex(e => _eqComparer.Equals(e, key));
                    _list[idx] = key;
                }
                else
                {
                    _dic[key] = value;
                    _list.Add(key);
                }
                _version++;
            }
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> 内のキーを格納しているコレクションを取得します。
        /// </summary>
        /// <value><see cref="OrderedDictionary{TKey, TValue}"/> 内のキーを格納している <see cref="KeyCollection"/>。</value>
        public KeyCollection Keys => _keys ?? (_keys = new KeyCollection(this));

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> 内の値を格納しているコレクションを取得します。
        /// </summary>
        /// <value><see cref="OrderedDictionary{TKey, TValue}"/> 内の値を格納している <see cref="ValueCollection"/>。</value>
        public ValueCollection Values => _values ?? (_values = new ValueCollection(this));

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        #endregion

        #region Methods

        /// <summary>指定したキーと値をディクショナリに追加します。</summary>
        /// <param name="key">追加する要素のキー。</param>
        /// <param name="value">追加する要素の値。 参照型の場合は null の値を使用できます。</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        /// <exception cref="ArgumentException">同じキーを持つ要素が、<see cref="OrderedDictionary{TKey, TValue}"/> に既に存在します。</exception>
        public void Add(TKey key, TValue value)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));
            ThrowIf(_dic.ContainsKey(key), () => new ArgumentException(nameof(key)));

            _list.Add(key);
            _dic.Add(key, value);
            _version++;
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> からすべてのキーと値を削除します。
        /// </summary>
        public void Clear()
        {
            _dic.Clear();
            _list.Clear();
            _version++;
        }

        /// <summary>
        /// 指定したキーが <see cref="OrderedDictionary{TKey, TValue}"/> に格納されているかどうかを判断します。
        /// </summary>
        /// <param name="key"><see cref="OrderedDictionary{TKey, TValue}"/> 内で検索されるキー。</param>
        /// <returns>指定したキーを持つ要素が <see cref="OrderedDictionary{TKey, TValue}"/> に格納されている場合は true。それ以外の場合は false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        public bool ContainsKey(TKey key)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

            return _dic.ContainsKey(key);
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> に特定の値が格納されているかどうかを判断します。
        /// </summary>
        /// <param name="value"><see cref="OrderedDictionary{TKey, TValue}"/> 内で検索される値。参照型の場合は null の値を使用できます。</param>
        /// <returns>指定した値を持つ要素が <see cref="OrderedDictionary{TKey, TValue}"/> に格納されている場合は true。それ以外の場合は false。</returns>
        public bool ContainsValue(TValue value) => _dic.ContainsValue(value);

        private void copyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            ThrowIf(array == null, () => new ArgumentNullException(nameof(array)));
            ThrowIf(index < 0 || array.Length <= index, () => new ArgumentNullException(nameof(index)));
            ThrowIf(array.Length - index < _list.Count, () => new ArgumentException());

            for (int i = 0; i < _list.Count; i++)
            {
                array[index++] = new KeyValuePair<TKey, TValue>(_list[i], _dic[_list[i]]);
            }
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> を反復処理する列挙子を返します。
        /// </summary>
        /// <returns><see cref="OrderedDictionary{TKey, TValue}"/> の <see cref="Enumerator"/> 構造体。</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// <see cref="ISerializable"/> インターフェイスを実装し、<see cref="OrderedDictionary{TKey, TValue}"/> インスタンスをシリアル化するために必要なデータを返します。
        /// </summary>
        /// <param name="info"><see cref="OrderedDictionary{TKey, TValue}"/> インスタンスをシリアル化するために必要な情報を格納する <see cref="SerializationInfo"/> オブジェクト。</param>
        /// <param name="context"><see cref="OrderedDictionary{TKey, TValue}"/> インスタンスに関連付けられているシリアル化ストリームの転送元および転送先を格納する <see cref="StreamingContext"/> 構造体。</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> は null です。</exception>
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ThrowIf(info == null, () => new ArgumentNullException(nameof(info)));

            info.AddValue(InitialCapacityName, _initialCapacity);
            info.AddValue(VersionName, _version);
            info.AddValue(EqualityComparerName, _eqComparer, typeof(IEqualityComparer<TKey>));
            if (_list.Count != 0)
            {
                var ary = _list.Select(e => new KeyValuePair<TKey, TValue>(e, _dic[e])).ToArray();
                info.AddValue(KeyValuePairsName, ary, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> コレクションの指定したインデックス位置に、指定したキーと値を持つ新しいエントリを挿入します。
        /// </summary>
        /// <param name="index">要素を挿入する位置の、0 から始まるインデックス番号。</param>
        /// <param name="key">追加するエントリのキー。</param>
        /// <param name="value">追加するエントリの値。値は null に設定できます。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> より大きくなっています。</exception>
        /// <exception cref="ArgumentException">同じキーを持つ要素がコレクションに既に存在しています。</exception>
        public void Insert(int index, TKey key, TValue value)
        {
            ThrowIf(index < 0 || _list.Count <= index, () => new ArgumentOutOfRangeException(nameof(index)));
            ThrowIf(_dic.ContainsKey(key), () => new ArgumentException(nameof(key)));

            _list.Insert(index, key);
            _dic[key] = value;
            _version++;
        }

        /// <summary>
        /// <see cref="ISerializable"/> インターフェイスを実装し、逆シリアル化が完了したときに逆シリアル化イベントを発生させます。
        /// </summary>
        /// <param name="sender">逆シリアル化イベントのソース。</param>
        /// <exception cref="SerializationException"><see cref="OrderedDictionary{TKey, TValue}"/> オブジェクトに現在関連付けられている <see cref="SerializationInfo"/> インスタンスが無効です。</exception>
        public virtual void OnDeserialization(object sender)
        {
            ThrowIf(_si == null, () => new SerializationException());

            _eqComparer = _si.GetValue(EqualityComparerName, typeof(IEqualityComparer<TKey>)) as IEqualityComparer<TKey>;
            _initialCapacity = _si.GetInt32(InitialCapacityName);
            var realVersion = _si.GetInt32(VersionName);
            var kvps = _si.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[])) as KeyValuePair<TKey, TValue>[] ?? new KeyValuePair<TKey, TValue>[] { };

            foreach (var e in kvps)
            {
                Add(e.Key, e.Value);
            }
            _version = realVersion;
        }

        /// <summary>
        /// 指定したキーを持つ値を <see cref="OrderedDictionary{TKey, TValue}"/> から削除します。
        /// </summary>
        /// <param name="key">削除する要素のキー。</param>
        /// <returns>要素が見つかり、正常に削除された場合は true。それ以外の場合は false。このメソッドは、<paramref name="key"/> が <see cref="OrderedDictionary{TKey, TValue}"/> に見つからない場合、false を返します。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        public bool Remove(TKey key)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

            if (_dic.Remove(key) && _list.Remove(key))
            {
                _version++;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 指定したインデックス位置にある要素を削除します。
        /// </summary>
        /// <param name="index">削除する要素の 0 から始まるインデックス。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> 以上になっています。</exception>
        public void RemoveAt(int index)
        {
            ThrowIf(index < 0 || _list.Count <= index, () => new ArgumentOutOfRangeException(nameof(index)));

            var k = _list[index];
            _dic.Remove(k);
            _list.RemoveAt(index);
            _version++;
        }

        /// <summary>
        /// 指定したキーに関連付けられている値を取得します。
        /// </summary>
        /// <param name="key">取得する値のキー。</param>
        /// <param name="value">このメソッドから制御が戻るときに、キーが見つかった場合は、指定したキーに関連付けられている値が格納されます。それ以外の場合は <paramref name="value"/> パラメーターの型に対する既定の値です。このパラメーターは初期化せずに渡されます。</param>
        /// <returns>指定したキーを持つ要素が <see cref="OrderedDictionary{TKey, TValue}"/> に格納されている場合は true。それ以外の場合は false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> は null です。</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

            if (_dic.ContainsKey(key))
            {
                value = _dic[key];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// 指定したインデックス位置にある要素に対応するキーを取得します。
        /// </summary>
        /// <param name="index">キーを取得する要素の 0 から始まるインデックス。</param>
        /// <returns>指定したインデックスにある要素に対応するキーの値。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。または <paramref name="index"/> が <see cref="Count"/> 以上になっています。</exception>
        public TKey KeyAt(int index)
        {
            ThrowIf(index < 0 || _list.Count <= index, () => new ArgumentOutOfRangeException(nameof(index)));

            return _list[index];
        }

        #endregion

        #region EIMI

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => _dic.ContainsKey(item.Key) && _dic[item.Key].Equals(item.Value);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            copyTo(array, index);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dic.ContainsKey(item.Key) && _dic[item.Key].Equals(item.Value))
            {
                return Remove(item.Key);
            }
            return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void IDictionary.Add(object key, object value)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));
            ThrowIf(value == null && default(TValue) != null, () => new ArgumentNullException(nameof(value))); // TValue is struct.

            try
            {
                var tkey = (TKey)key;
                try
                {
                    Add(tkey, (TValue)value);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(nameof(value));
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(nameof(key));
            }
        }

        bool IDictionary.Contains(object key)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

            return key is TKey ? ContainsKey((TKey)key) : false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this);

        void IDictionary.Remove(object key)
        {
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));

            if (key is TKey)
            {
                Remove((TKey)key);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIf(array == null, () => new ArgumentNullException(nameof(array)));
            ThrowIf(array.Rank != 1, () => new ArgumentException(nameof(array)));
            ThrowIf(array.GetLowerBound(0) != 0, () => new ArgumentException(nameof(array)));
            ThrowIf(index < 0 || array.Length <= index, () => new ArgumentOutOfRangeException(nameof(index)));
            ThrowIf(array.Length - index < _list.Count, () => new ArgumentException());

            var kvps = array as KeyValuePair<TKey, TValue>[];
            if (kvps != null)
            {
                copyTo(kvps, index);
            }
            else if (array is DictionaryEntry[])
            {
                var deary = array as DictionaryEntry[];
                for (int i = 0; i < _list.Count; i++)
                    deary[index++] = new DictionaryEntry(_list[i], _dic[_list[i]]);
            }
            else
            {
                var objs = array as object[];
                if (objs == null)
                    throw new ArgumentException();

                try
                {
                    for (int i = 0; i < _list.Count; i++)
                        objs[index++] = new KeyValuePair<TKey, TValue>(_list[i], _dic[_list[i]]);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        object IDictionary.this[object key]
        {
            get
            {
                ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));
                var k = (TKey)key;
                if (_dic.ContainsKey(k))
                {
                    return _dic[k];
                }
                return null;
            }
            set
            {
                ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));
                ThrowIf(value == null && default(TValue) != null, () => new ArgumentNullException(nameof(value))); // TValue is struct.

                try
                {
                    var k = (TKey)key;
                    try
                    {
                        this[k] = (TValue)value;
                        _version++;
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException(nameof(value));
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(nameof(key));
                }
            }
        }

        object IOrderedDictionary.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                ThrowIf(value == null && default(TValue) != null, () => new ArgumentNullException(nameof(value))); // TValue is struct.
                try
                {
                    this[index] = (TValue)value;
                    _version++;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(nameof(value));
                }
            }
        }

        void IOrderedDictionary.Insert(int index, object key, object value)
        {
            ThrowIf(index < 0 || _list.Count <= index, () => new ArgumentOutOfRangeException(nameof(index)));
            ThrowIf(key == null, () => new ArgumentNullException(nameof(key)));
            ThrowIf(value == null && default(TValue) != null, () => new ArgumentNullException(nameof(value))); // TValue is struct.
            try
            {
                var k = (TKey)key;
                ThrowIf(_dic.ContainsKey(k), () => new ArgumentException(nameof(key)));

                try
                {
                    var v = (TValue)value;
                    Insert(index, k, v);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(nameof(value));
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(nameof(key));
            }
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator() => GetEnumerator();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="create"></param>
        private static void ThrowIf(bool condition, Func<Exception> create)
        {
            if (condition)
            {
                throw create();
            }
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> の要素を列挙します。
        /// </summary>
        [Serializable]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private OrderedDictionary<TKey, TValue> dic;
            private KeyValuePair<TKey, TValue> current;
            private int index;
            private int version;

            internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
            {
                dic = dictionary;
                index = 0;
                version = dictionary._version;
                current = new KeyValuePair<TKey, TValue>();
            }

            /// <summary>
            /// 列挙子の現在位置の要素を取得します。
            /// </summary>
            /// <value><see cref="OrderedDictionary{TKey, TValue}"/> のうち、列挙子の現在位置にある要素。</value>
            public KeyValuePair<TKey, TValue> Current => current;

            /// <summary>
            /// <see cref="Enumerator"/> によって使用されているすべてのリソースを解放します。
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// 列挙子を <see cref="OrderedDictionary{TKey, TValue}"/> の次の要素に進めます。
            /// </summary>
            /// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
            /// <exception cref="InvalidOperationException">コレクションは、列挙子の作成後に変更されました。</exception>
            public bool MoveNext()
            {
                if (version != dic._version)
                {
                    throw new InvalidOperationException();
                }

                while ((uint)index < (uint)dic.Count)
                {
                    var k = dic._list[index];
                    current = new KeyValuePair<TKey, TValue>(k, dic._dic[k]);
                    index++;
                    return true;
                }
                index = dic.Count + 1;
                current = new KeyValuePair<TKey, TValue>();
                return false;
            }

            void IEnumerator.Reset()
            {
                if (version != dic._version)
                {
                    throw new InvalidOperationException();
                }

                index = 0;
                current = new KeyValuePair<TKey, TValue>();
            }

            object IEnumerator.Current => Current;

            object IDictionaryEnumerator.Key => Current.Key;

            object IDictionaryEnumerator.Value => Current.Value;

            DictionaryEntry IDictionaryEnumerator.Entry => new DictionaryEntry(Current.Key, Current.Value);
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> 内のキーのコレクションを表します。 このクラスは継承できません。
        /// </summary>
        [Serializable]
        public sealed class KeyCollection : ICollection<TKey>, IReadOnlyCollection<TKey>, ICollection
        {
            private OrderedDictionary<TKey, TValue> d;

            /// <summary>
            /// 指定した <see cref="OrderedDictionary{TKey, TValue}"/> 内のキーを反映する、<see cref="KeyCollection"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="dictionary">新しい <see cref="KeyCollection"/> にキーが反映される <see cref="OrderedDictionary{TKey, TValue}"/>。</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
            public KeyCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException(nameof(dictionary));
                }

                d = dictionary;
            }

            /// <summary>
            /// <see cref="KeyCollection"/> を反復処理する列挙子を返します。
            /// </summary>
            /// <returns><see cref="KeyCollection"/> の <see cref="Enumerator"/>。</returns>
            public Enumerator GetEnumerator() => new Enumerator(d);

            /// <summary>
            /// <see cref="KeyCollection"/> の要素を既存の 1 次元の <see cref="Array"/> にコピーします。コピー操作は、配列内の指定したインデックスから始まります。
            /// </summary>
            /// <param name="array"><see cref="KeyCollection"/> から要素がコピーされる 1 次元の <see cref="Array"/>。<see cref="Array"/>には、0 から始まるインデックス番号が必要です。</param>
            /// <param name="index">コピーの開始位置とする <paramref name="array"/> のインデックス (0 から始まる)。</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> は null です。</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。</exception>
            /// <exception cref="ArgumentException">コピー元の <see cref="KeyCollection"/> の要素数が、<paramref name="index"/> からコピー先の <paramref name="array"/> の末尾までに格納できる数を超えています。</exception>
            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }
                if (index < 0 || array.Length <= index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (array.Length - index < d._list.Count)
                {
                    throw new ArgumentException();
                }

                d._list.CopyTo(array, index);
            }

            /// <summary>
            /// <see cref="KeyCollection"/> に格納されている要素の数を取得します。
            /// </summary>
            /// <value><see cref="KeyCollection"/> に格納されている要素の数。このプロパティ値を取得することは、O(1) 操作になります。</value>
            public int Count => d._list.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Contains(TKey item) => d._dic.ContainsKey(item);

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }
                if (array.Rank != 1)
                {
                    throw new ArgumentException(nameof(array));
                }
                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException(nameof(array));
                }
                if (index < 0 || array.Length <= index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (array.Length - index < d._list.Count)
                {
                    throw new ArgumentException();
                }

                var keys = array as TKey[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    var objs = array as object[];
                    if (objs == null)
                    {
                        throw new ArgumentException();
                    }
                    try
                    {
                        for (int i = 0; i < d._list.Count; i++)
                        {
                            objs[index++] = d._list[i];
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException();
                    }
                }
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => (d as ICollection).SyncRoot;

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new Enumerator(d);

            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(d);

            /// <summary>
            /// <see cref="KeyCollection"/> の要素を列挙します。
            /// </summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private OrderedDictionary<TKey, TValue> d;
                private int index;
                private TKey current;
                private int version;

                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
                {
                    d = dictionary;
                    index = 0;
                    version = dictionary._version;
                    current = default(TKey);
                }

                /// <summary>
                /// <see cref="Enumerator"/> によって使用されているすべてのリソースを解放します。
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// 列挙子を <see cref="KeyCollection"/> の次の要素に進めます。
                /// </summary>
                /// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
                /// <exception cref="InvalidOperationException">コレクションは、列挙子の作成後に変更されました。</exception>
                public bool MoveNext()
                {
                    if (d._version != version)
                    {
                        throw new InvalidOperationException();
                    }

                    while ((uint)index < (uint)d._list.Count)
                    {
                        current = d._list[index];
                        index++;
                        return true;
                    }

                    index = d._list.Count + 1;
                    current = default(TKey);
                    return false;
                }

                /// <summary>
                /// 列挙子の現在位置の要素を取得します。
                /// </summary>
                /// <value><see cref="KeyCollection"/> のうち、列挙子の現在位置にある要素。</value>
                public TKey Current => current;

                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || index == d._list.Count + 1)
                        {
                            throw new InvalidOperationException();
                        }
                        return current;
                    }
                }

                void IEnumerator.Reset()
                {
                    index = 0;
                    current = default(TKey);
                }
            }
        }

        /// <summary>
        /// <see cref="OrderedDictionary{TKey, TValue}"/> の値のコレクションを表します。 このクラスは継承できません。
        /// </summary>
        [Serializable]
        public sealed class ValueCollection : ICollection<TValue>, IReadOnlyCollection<TValue>, ICollection
        {
            private OrderedDictionary<TKey, TValue> d;

            /// <summary>
            /// 指定した <see cref="OrderedDictionary{TKey, TValue}"/> 内の値を反映する、<see cref="ValueCollection"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="dictionary">新しい <see cref="ValueCollection"/> にキーが反映される <see cref="OrderedDictionary{TKey, TValue}"/>。</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> は null です。</exception>
            public ValueCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException(nameof(dictionary));
                }

                d = dictionary;
            }

            /// <summary>
            /// <see cref="ValueCollection"/> を反復処理する列挙子を返します。
            /// </summary>
            /// <returns><see cref="ValueCollection"/> の <see cref="Enumerator"/>。</returns>
            public Enumerator GetEnumerator() => new Enumerator(d);

            /// <summary>
            /// <see cref="ValueCollection"/> の要素を既存の 1 次元の <see cref="Array"/> にコピーします。コピー操作は、配列内の指定したインデックスから始まります。
            /// </summary>
            /// <param name="array"><see cref="ValueCollection"/> から要素がコピーされる 1 次元の <see cref="Array"/>。<see cref="Array"/>には、0 から始まるインデックス番号が必要です。</param>
            /// <param name="index">コピーの開始位置とする <paramref name="array"/> のインデックス (0 から始まる)。</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> は null です。</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> が 0 未満です。</exception>
            /// <exception cref="ArgumentException">コピー元の <see cref="ValueCollection"/> の要素数が、<paramref name="index"/> からコピー先の <paramref name="array"/> の末尾までに格納できる数を超えています。</exception>
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }
                if (index < 0 || array.Length <= index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (array.Length - index < d._list.Count)
                {
                    throw new ArgumentException();
                }

                for (int i = 0; i < d._list.Count; i++)
                {
                    array[index++] = d._dic[d._list[i]];
                }
            }

            /// <summary>
            /// <see cref="ValueCollection"/> に格納されている要素の数を取得します。
            /// </summary>
            /// <value><see cref="ValueCollection"/> に格納されている要素の数。</value>
            public int Count => d._list.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Contains(TValue item) => d._dic.ContainsValue(item);

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }
                if (array.Rank != 1)
                {
                    throw new ArgumentException(nameof(array));
                }
                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException(nameof(array));
                }
                if (index < 0 || array.Length <= index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (array.Length - index < d._list.Count)
                {
                    throw new ArgumentException();
                }

                var vals = array as TValue[];
                if (vals != null)
                {
                    CopyTo(vals, index);
                }
                else
                {
                    var objs = array as object[];
                    if (objs == null)
                    {
                        throw new ArgumentException();
                    }

                    try
                    {
                        for (int i = 0; i < d._list.Count; i++)
                        {
                            objs[index++] = d._dic[d._list[i]];
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException();
                    }
                }
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => (d as ICollection).SyncRoot;

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new Enumerator(d);

            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(d);

            /// <summary>
            /// <see cref="ValueCollection"/> の要素を列挙します。
            /// </summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TValue>, IEnumerator
            {
                private OrderedDictionary<TKey, TValue> _d;
                private int _index;
                private TValue _current;
                private int _version;

                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
                {
                    _d = dictionary;
                    _index = 0;
                    _version = dictionary._version;
                    _current = default(TValue);
                }

                /// <summary>
                /// <see cref="Enumerator"/> によって使用されているすべてのリソースを解放します。
                /// </summary>
                public void Dispose()
                {
                }

                /// <summary>
                /// <see cref="ValueCollection"/> の次の要素に進めます。
                /// </summary>
                /// <returns>列挙子が次の要素に正常に進んだ場合は true。列挙子がコレクションの末尾を越えた場合は false。</returns>
                /// <exception cref="InvalidOperationException">コレクションは、列挙子の作成後に変更されました。</exception>
                public bool MoveNext()
                {
                    if (_d._version != _version)
                    {
                        throw new InvalidOperationException();
                    }

                    while ((uint)_index < (uint)_d._list.Count)
                    {
                        _current = _d._dic[_d._list[_index]];
                        _index++;
                        return true;
                    }

                    _index = _d._list.Count + 1;
                    _current = default(TValue);
                    return false;
                }

                /// <summary>
                /// 列挙子の現在位置の要素を取得します。
                /// </summary>
                /// <value><see cref="ValueCollection"/> のうち、列挙子の現在位置にある要素。</value>
                public TValue Current => _current;

                object IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || (uint)_index == (uint)_d._list.Count + 1u)
                        {
                            throw new InvalidOperationException();
                        }

                        return _current;
                    }
                }

                void IEnumerator.Reset()
                {
                    _index = 0;
                    _current = default(TValue);
                }
            }
        }
    }
}
