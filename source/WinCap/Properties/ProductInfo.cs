using System;
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
        private static readonly Lazy<string> _versionLazy = new Lazy<string>(() => $"{Version.ToString(3)}{(Version.Revision == 0 ? "" : " rev." + Version.Revision)}");

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
        public static string VersionString => _versionLazy.Value;
    }
}
