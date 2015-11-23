using System;
using System.Reflection;

namespace WinCap.Models
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

        /// <summary>
        /// タイトル
        /// </summary>
        public static string Title => titleLazy.Value;

        /// <summary>
        /// 説明
        /// </summary>
        public static string Description => descriptionLazy.Value;

        /// <summary>
        /// 会社
        /// </summary>
        public static string Company => companyLazy.Value;

        /// <summary>
        /// 製品
        /// </summary>
        public static string Product => productLazy.Value;

        /// <summary>
        /// 著作権
        /// </summary>
        public static string Copyright => copyrightLazy.Value;

        /// <summary>
        /// 商標
        /// </summary>
        public static string Trademark => trademarkLazy.Value;

        /// <summary>
        /// バージョン
        /// </summary>
        public static Version Version => assembly.GetName().Version;
    }
}
