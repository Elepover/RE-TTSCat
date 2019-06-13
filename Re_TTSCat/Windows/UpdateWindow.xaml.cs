using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Re_TTSCat.Data;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
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
                IsEnabled = false;
                TextBlock_Status.Text = "正在检查更新...";
                ProgressBar_Indicator.Visibility = Visibility.Visible;
                var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                var currentVersion = Vars.currentVersion;
                TextBlock_Latest.Text = "最新版本 (Stable): " + latestVersion.LatestVersion.ToString() + " / 当前版本: " + currentVersion.ToString();
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
                IsEnabled = true;
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

        bool _shown = false;

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;

            await CheckUpdate();
        }
    }
}
