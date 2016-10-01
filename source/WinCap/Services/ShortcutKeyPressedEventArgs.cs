using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinCap.Services
{
    public class ShortcutKeyPressedEventArgs
    {
        public ShortcutKey ShortcutKey { get; }

        public bool Handled { get; set; }

        public ShortcutKeyPressedEventArgs(ShortcutKey shortcutKey)
        {
            this.ShortcutKey = shortcutKey;
        }

        internal ShortcutKeyPressedEventArgs(Keys key, ICollection<Keys> modifiers)
        {
            this.ShortcutKey = new ShortcutKey(key, modifiers);
        }
    }
}
