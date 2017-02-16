using System;
using System.Collections.Generic;
using System.Reflection;

namespace WinCap.Properties
{
    /// <summary>
    /// 製品情報を提供します。
    /// </summary>
    public static class ProductInfo
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static readonly Lazy<string> titleLazy = new Lazy<string>(() => ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute))).Title);
        private static readonly Lazy<string> descriptionLazy = new Lazy<string>(() => ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute))).Description);
        private static readonly Lazy<string> companyLazy = new Lazy<string>(() => ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute))).Company);
        private static readonly Lazy<string> productLazy = new Lazy<string>(() => ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute))).Product);
        private static readonly Lazy<string> copyrightLazy = new Lazy<string>(() => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))).Copyright);
        private static readonly Lazy<string> trademarkLazy = new Lazy<string>(() => ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTrademarkAttribute))).Trademark);
        private static readonly Lazy<string> versionLazy = new Lazy<string>(() => $"{Version.ToString(3)}{(Version.Revision == 0 ? "" : " rev." + Version.Revision)}");
        private static readonly Lazy<IReadOnlyCollection<Library>> librariesLazy = new Lazy<IReadOnlyCollection<Library>>(() => new List<Library>
        {
            new Library("Open.WinKeyboardHook", new Uri("https://github.com/lontivero/Open.WinKeyboardHook")),
            new Library("MetroRadiance", new Uri("https://github.com/Grabacr07/MetroRadiance")),
            new Library("Livet", new Uri("http://ugaya40.hateblo.jp/entry/Livet")),
        });

        /// <summary>
        /// タイトルを取得します。
        /// </summary>
        public static string Title => titleLazy.Value;

        /// <summary>
        /// 説明を取得します。
        /// </summary>
        public static string Description => descriptionLazy.Value;

        /// <summary>
        /// 会社を取得します。
        /// </summary>
        public static string Company => companyLazy.Value;

        /// <summary>
        /// 製品を取得します。
        /// </summary>
        public static string Product => productLazy.Value;

        /// <summary>
        /// 著作権を取得します。
        /// </summary>
        public static string Copyright => copyrightLazy.Value;

        /// <summary>
        /// 商標を取得します。
        /// </summary>
        public static string Trademark => trademarkLazy.Value;

        /// <summary>
        /// バージョンを取得します。
        /// </summary>
        public static Version Version => assembly.GetName().Version;

        /// <summary>
        /// バージョン文字列を取得します。
        /// </summary>
        public static string VersionString => versionLazy.Value;

        /// <summary>
        /// ライブラリを取得します。
        /// </summary>
        public static IReadOnlyCollection<Library> Libraries => librariesLazy.Value;
    }

    /// <summary>
    /// ライブラリ情報を提供します。
    /// </summary>
    public class Library
    {
        /// <summary>
        /// ライブラリ名を取得します。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// ライブラリのURLを取得します。
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="name">ライブラリ名</param>
        /// <param name="url">ライブラリのURL</param>
        public Library(string name, Uri url)
        {
            this.Name = name;
            this.Url = url;
        }
    }
}
