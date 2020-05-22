using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Re_TTSCat.Data;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Popup
    {
        public UpdateWindow()
        {
            InitializeComponent();
        }

        private string updateDownloadURL = "undefined";

        private async Task CheckUpdate()
        {
            try
            {
                MasterGrid.IsEnabled = false;
                TextBlock_Status.Text = "正在检查更新...";
                ProgressBar_Indicator.Visibility = Visibility.Visible;
                var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                var currentVersion = Vars.CurrentVersion;
                TextBlock_Latest.Text = "最新版本: " + latestVersion.LatestVersion.ToString() + " / 当前版本: " + currentVersion.ToString();
                if (KruinUpdates.CheckIfLatest(latestVersion, currentVersion))
                {
                    TextBlock_Status.Text = "插件已为最新";
                }
                else
                {
                    TextBlock_Status.Text = "发现更新";
                }
                TextBox_UpdContents.Text = "更新时间: " + latestVersion.UpdateTime.ToString() + "\n" + "更新日志: " + "\n" + latestVersion.UpdateDescription.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                updateDownloadURL = "https://www.danmuji.org" + latestVersion.DownloadLink;
            }
            catch (Exception ex)
            {
                TextBlock_Latest.Text = "最新版本: 未知";
                TextBox_UpdContents.Text = "更新出错: " + ex.ToString();
                TextBlock_Status.Text = "更新出错";
            }
            finally
            {
                ProgressBar_Indicator.Visibility = Visibility.Hidden;
                MasterGrid.IsEnabled = true;
            }
        }

        private async void Button_CheckUpd_Click(object sender, RoutedEventArgs e)
        {
            await CheckUpdate();
        }

        private void Button_DLUpd_Click(object sender, RoutedEventArgs e)
        {
            if (updateDownloadURL == "undefined")
            {
                AsyncDialog.Open("先检查更新吧~");
            }
            else
            {
                Process.Start(updateDownloadURL);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        private async void Popup_Opened(object sender, EventArgs e)
        {
            await CheckUpdate();
        }
    }
}
