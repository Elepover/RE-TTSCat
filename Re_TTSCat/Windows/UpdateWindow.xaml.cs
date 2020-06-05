using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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

        private void DetectPendingUpdates()
        {
            Button_DLUpd.IsEnabled = !Vars.UpdatePending;
            Button_CheckUpd.IsEnabled = !Vars.UpdatePending;
            if (Vars.UpdatePending)
            {
                TextBlock_Status.Text = "已安装更新，等待弹幕姬重启...";
                TextBlock_Status.FontWeight = FontWeights.SemiBold;
                TextBlock_Status.Foreground = new SolidColorBrush(Colors.DarkGreen);
                FontIcon_DlUpdate.Foreground = new SolidColorBrush(Colors.DarkGreen);
                FontIcon_DlUpdate.Text = "\uF00C";
                FontIcon_Title.Text = "\uF00C";
            }
        }

        private async Task CheckUpdate()
        {
            try
            {
                MasterGrid.IsEnabled = false;
                TextBlock_Status.Text = "正在检查更新...";
                ProgressBar_Indicator.Visibility = Visibility.Visible;
                var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                var currentVersion = Vars.CurrentVersion;
                TextBlock_Latest.Text = $"最新版本: {latestVersion.LatestVersion} / 当前版本: {currentVersion}";
                if (currentVersion > latestVersion.LatestVersion)
                    TextBlock_Latest.Text += "（草，怎么你的版本还更新）"; // <- 不愧是我 2/1
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
                DetectPendingUpdates();
                ProgressBar_Indicator.Visibility = Visibility.Hidden;
                MasterGrid.IsEnabled = true;
            }
        }

        private async void Button_CheckUpd_Click(object sender, RoutedEventArgs e)
        {
            await CheckUpdate();
        }

        private async void Button_DLUpd_Click(object sender, RoutedEventArgs e)
        {
            if (updateDownloadURL == "undefined")
            {
                AsyncDialog.Open("先检查更新吧~", "Re: TTSCat", System.Windows.Forms.MessageBoxIcon.Warning, System.Windows.Forms.MessageBoxButtons.OK);
            }
            else
            {
                try
                {
                    StaysOpen = true;
                    ProgressBar_Indicator.Visibility = Visibility.Visible;
                    ProgressBar_Indicator.IsIndeterminate = false;
                    Button_CheckUpd.IsEnabled = false;
                    Button_DLUpd.IsEnabled = false;

                    TextBlock_Status.Text = "正在准备更新...";
                    ProgressBar_Indicator.Value = 0;
                    await Task.Delay(100);

                    TextBlock_Status.Text = "正在下载更新...";
                    ProgressBar_Indicator.Value = 10;

                    void progressChangedHandler(object sdr, DownloadProgressChangedEventArgs dpce)
                    {
                        var progress = Math.Round((double)dpce.BytesReceived / dpce.TotalBytesToReceive, 4);
                        TextBlock_Status.Text = $"正在下载更新... ({progress * 100}%)";
                        ProgressBar_Indicator.Value = progress * 100;
                    }

                    using (var downloader = new WebClient())
                    {
                        downloader.DownloadProgressChanged += progressChangedHandler;
                        await downloader.DownloadFileTaskAsync(updateDownloadURL, Vars.DownloadUpdateFilename);
                    }

                    TextBlock_Status.Text = "正在备份...";
                    ProgressBar_Indicator.Value = 50;
                    var backupFilename = Path.Combine(Vars.ConfDir, $"Re_TTSCat_v{Vars.CurrentVersion}.dll");
                    if (File.Exists(backupFilename)) File.Delete(backupFilename);
                    File.Move(Vars.AppDllFileName, backupFilename);

                    TextBlock_Status.Text = "正在解压...";
                    ProgressBar_Indicator.Value = 80;
                    using (var zip = ZipFile.OpenRead(Vars.DownloadUpdateFilename))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            TextBlock_Status.Text = $"正在解压... ({entry.FullName})";
                            entry.ExtractToFile(Path.Combine(Vars.AppDllFilePath, entry.FullName), true);
                        }
                    }

                    TextBlock_Status.Text = "正在清理...";
                    ProgressBar_Indicator.Value = 90;
                    if (File.Exists(Vars.DownloadUpdateFilename)) File.Delete(Vars.DownloadUpdateFilename);

                    Vars.UpdatePending = true;
                    ProgressBar_Indicator.Value = 100;
                }
                catch (Exception ex)
                {
                    TextBlock_Status.Text = $"更新出错: {ex.Message}";
                }
                finally
                {
                    StaysOpen = false;
                    ProgressBar_Indicator.Visibility = Visibility.Hidden;
                    ProgressBar_Indicator.IsIndeterminate = true;
                    Button_CheckUpd.IsEnabled = true;
                    Button_DLUpd.IsEnabled = !Vars.UpdatePending;
                    DetectPendingUpdates();
                }
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
