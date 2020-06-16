using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// TestVoiceReplyParamsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestVoiceReplyParamsWindow : Window
    {
        private bool stayOpen = false;
        private bool isOkClicked = false;
        private bool isClosed = false;
        private bool isComponentsReady = false;
        public TestVoiceReplyParamsWindow()
        {
            InitializeComponent();
            isComponentsReady = true;
        }
        public static async Task<DanmakuModel> GetDanmakuModel(string content)
        {
            var window = new TestVoiceReplyParamsWindow();
            window.template = content;
            window.UpdatePreview(null, null);
            window.Show();
            while (!window.isClosed) await Task.Delay(100);
            if (!window.isOkClicked) return null;
            return window.model;
        }

        private string template;

        private DanmakuModel model
        {
            get => new DanmakuModel()
            {
                CommentText = TextBox_DanmakuContent.Text,
                GiftName = TextBox_GiftName.Text,
                GiftCount = int.Parse(TextBox_GiftQty.Text),
                UserName = TextBox_Username.Text,
                isVIP = CheckBox_IsUserVip.IsChecked ?? true,
                UserGuardLevel = guardLevel,
                MsgType = (RadioButton_Danmaku.IsChecked ?? true) ? MsgTypeEnum.Comment : MsgTypeEnum.GiftSend
            };
        }

        private int guardLevel
        {
            get
            {
                if (RadioButton_1.IsChecked == true) return 1;
                if (RadioButton_2.IsChecked == true) return 2;
                if (RadioButton_3.IsChecked == true) return 3;
                return 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isOkClicked = true;
            isClosed = true;
            Close();
        }

        private void TextBox_GiftQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isComponentsReady) return;
            if (!(sender is TextBox)) return;
            try
            {
                _ = int.Parse((sender as TextBox).Text);
                (sender as TextBox).Background = new SolidColorBrush(Colors.LightGreen);
                Button_Ok.IsEnabled = true;
            }
            catch
            {
                (sender as TextBox).Background = new SolidColorBrush(Colors.LightPink);
                Button_Ok.IsEnabled = false;
            }
            UpdatePreview(null, null);
        }

        private void UpdatePreview(object sender, TextChangedEventArgs e)
        {
            if (!isComponentsReady) return;
            if (model == null) return;
            try
            {
                string processed;
                if (RadioButton_Danmaku.IsChecked ?? true)
                {
                    processed = Main.ProcessDanmaku(model, template);
                }
                else
                {
                    processed = Main.ProcessGift(model, template);
                }
                TextBox_Preview.Text = processed;
            }
            catch (Exception ex)
            {
                TextBox_Preview.Text = $"处理失败: {ex.Message}{Environment.NewLine}{Environment.NewLine}技术详细信息:{Environment.NewLine}{ex}";
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview(null, null);
        }

        private void Button_KeepWindowOpen_Click(object sender, RoutedEventArgs e)
        {
            stayOpen = (sender as CheckBox)?.IsChecked ?? false;
            if (stayOpen)
            {
                ShowInTaskbar = true;
            }
            else
            {
                ShowInTaskbar = false;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                if (!stayOpen)
                {
                    isClosed = true;
                    Close();
                }
            }
            catch { }
        }
    }
}
