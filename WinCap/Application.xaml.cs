using Elysium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WinCap
{
    /// <summary>
    /// Application.xaml の相互作用ロジック
    /// </summary>
    public partial class Application : System.Windows.Application
    {
        private void StartupHandler(object sender, System.Windows.StartupEventArgs e)
        {
            Manager.Apply(this, Theme.Dark);
        }
    }
}
