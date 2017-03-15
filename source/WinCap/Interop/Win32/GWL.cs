namespace WinCap.Interop.Win32
{
    public enum GWL : int
    {
        /// <summary>Sets a new address for the window procedure.</summary>
        /// <remarks>You cannot change this attribute if the window does not belong to the same process as the calling thread.</remarks>
        WNDPROC = -4,

        /// <summary>Sets a new application instance handle.</summary>
        //GWLP_HINSTANCE = -6,

        //GWLP_HWNDPARENT = -8,

        /// <summary>Sets a new identifier of the child window.</summary>
        /// <remarks>The window cannot be a top-level window.</remarks>
        ID = -12,

        /// <summary>Sets a new window style.</summary>
        STYLE = -16,

        /// <summary>Sets a new extended window style.</summary>
        /// <remarks>See <see cref="ExWindowStyles"/>.</remarks>
        EXSTYLE = -20,

        /// <summary>Sets the user data associated with the window.</summary>
        /// <remarks>This data is intended for use by the application that created the window. Its value is initially zero.</remarks>
        USERDATA = -21,
    }
}
