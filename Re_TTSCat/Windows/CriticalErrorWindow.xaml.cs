using System.Windows;
using System.Windows.Media;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// CriticalErrorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CriticalErrorWindow : Window
    {
        public CriticalErrorWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TextBox_ErrorDetails.Text);
                TextBlock_Copied.Visibility = Visibility.Visible;
            }
            catch { }
        }

        private void Xpander_Collapsed(object sender, RoutedEventArgs e)
        {
            Xpander.RenderTransform = new TranslateTransform(0, 0);
            Height -= 175;
        }

        private void Xpander_Expanded(object sender, RoutedEventArgs e)
        {
            Xpander.RenderTransform = new TranslateTransform(0, -175);
            Height += 175;
        }
    }
}
