using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// DonateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DonateWindow : Popup
    {
        public DonateWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto:re-ttscat@itsmy.app");
        }
    }
}
