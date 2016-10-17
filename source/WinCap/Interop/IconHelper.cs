using System;
using System.Drawing;

namespace WinCap.Interop
{
    public static class IconHelper
    {
        /// <summary>
        /// リソースからアイコンを取得します。
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>アイコン</returns>
        public static Icon GetIconFromResource(Uri uri)
        {
            var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
            if (streamResourceInfo == null) throw new ArgumentException("Resource not found.", nameof(uri));

            using (var stream = streamResourceInfo.Stream)
            {
                return new Icon(stream, new Size(16, 16));
            }
        }
    }
}
