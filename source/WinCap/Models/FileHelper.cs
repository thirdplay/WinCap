using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WinCap.Models
{
    /// <summary>
    /// ファイルの補助機能を提供します。
    /// </summary>
    internal static class FileHelper
    {
        /// <summary>
        /// ファイル名パターンに従い、ファイル名を作成します。
        /// <para>%y:年</para>
        /// <para>%m:月</para>
        /// <para>%d:日</para>
        /// <para>%h:時</para>
        /// <para>%n:分</para>
        /// <para>%s:秒</para>
        /// <para>%c:連番</para>
        /// </summary>
        /// <param name="outputFolder">出力フォルダ</param>
        /// <param name="fileExtension">出力ファイル拡張子</param>
        /// <param name="fileNamePattern">ファイル名パターン</param>
        /// <param name="no">連番</param>
        /// <returns>ファイル名</returns>
        public static string CreateFileName(string outputFolder, string fileExtension, string fileNamePattern, int? no = null)
        {
            const string formatNo = "000";
            var replaceMapDateTime = new Dictionary<string, string>()
            {
                {FileNamePattern.Year, "yyyy"},
                {FileNamePattern.Month, "MM"},
                {FileNamePattern.Day, "dd"},
                {FileNamePattern.Hour, "HH"},
                {FileNamePattern.Minute, "mm"},
                {FileNamePattern.Second, "ss"},
            };
            string fileName = fileNamePattern;

            DateTime now = DateTime.Now;
            foreach (var pair in replaceMapDateTime)
            {
                if (fileName.Contains(pair.Key))
                {
                    fileName = Regex.Replace(fileName, pair.Key, now.ToString(pair.Value));
                }
            }

            // ファイル名パターンに連番がある場合
            if (fileNamePattern.Contains(FileNamePattern.Sequence))
            {
                // 連番指定がない場合
                if (!no.HasValue)
                {
                    // 有効な出力フォルダが指定されている場合、ファイルが存在しないファイル名を求める
                    no = 1;
                    if (!string.IsNullOrEmpty(outputFolder) && Directory.Exists(outputFolder))
                    {
                        List<string> list = new List<string>(Directory.GetFiles(outputFolder));
                        while (list.Contains(Path.Combine(outputFolder, Regex.Replace(fileName, FileNamePattern.Sequence, no.Value.ToString(formatNo))) + fileExtension))
                        {
                            no++;
                        }
                    }
                }
                fileName = Regex.Replace(fileName, FileNamePattern.Sequence, no.Value.ToString(formatNo));
            }

            return fileName;
        }
    }
}
