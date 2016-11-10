namespace WinCap.Interop
{
    /// <summary>
    /// 仮想キー変換の種別を表します。
    /// </summary>
    public enum MAPVK : uint
    {
        /// <summary>
        /// 仮想キーコードからスキャンコードに変換
        /// </summary>
        VK_TO_VSC = 0x00,

        /// <summary>
        /// スキャンコードから左右を区別しない仮想キーコードに変換
        /// </summary>
        VSC_TO_VK = 0x01,

        /// <summary>
        /// 仮想キーコードから戻り値の下位ワードにシフトなし文字値として変換
        /// </summary>
        VK_TO_CHAR = 0x02,

        /// <summary>
        ///スキャンコードから左右を区別する仮想キーコードに変換
        /// </summary>
        VSC_TO_VK_EX = 0x03,
        VK_TO_VSC_EX = 0x04,
    }
}
