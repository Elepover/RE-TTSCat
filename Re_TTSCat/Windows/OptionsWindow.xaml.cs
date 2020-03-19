using System;
using System.Threading.Tasks;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Net;
using System.Windows.Threading;
using System.Threading;
using Re_TTSCat.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// OptionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        private Thread StatsUpdater;
        private bool WindowClosed = true;
        public bool WindowDisposed = false;
        private UpdateWindow updateWindow;

        private void Button_CheckConnectivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var thread = new Thread(() =>
                {
                    var window = new LoadingWindowLight()
                    {
                        Left = System.Windows.Forms.Cursor.Position.X,
                        Top = System.Windows.Forms.Cursor.Position.Y
                    };
                    window.Show();
                    var result = new StringBuilder();
                    var listPing = new List<List<long>>();
                    var listAddresses = new Dictionary<string, string>
                    {
                        { "Google", "https://translate.google.cn/" },
                        { "百度", "https://fanyi.baidu.com/" },
                        { "有道", "http://tts.youdao.com/" }
                    };
                    var sw = new Stopwatch();
                    int i = 0;
                    foreach (var item in listAddresses)
                    {
                        window.ProgressBar.Value = (double)(i * 100) / listAddresses.Count;
                        var list = new List<long>();
                        for (int j = 0; j < 10; j++)
                        {
                            var req = WebRequest.CreateHttp(item.Value);
                            req.Timeout = 5000;
                            var frame = new DispatcherFrame();
                            var getResWorker = new Thread(() =>
                            {
                                try
                                {
                                    using (var res = req.GetResponse()) { }
                                }
                                catch { }
                                frame.Continue = false;
                            });
                            sw.Restart();
                            getResWorker.Start();
                            Dispatcher.PushFrame(frame);
                            sw.Stop();
                            list.Add(sw.ElapsedMilliseconds);
                            window.ProgressBar.Value += (double)(100 / listAddresses.Count) / 10;
                        }
                        listPing.Add(list);
                        i++;
                    }
                    window.ProgressBar.IsIndeterminate = true;
                    // process data
                    _ = result.Append($"服务器 / 延迟(ms) / 平均值 / 最小 / 最大 / 标准差{Environment.NewLine}");
                    i = 0;
                    foreach (var item in listAddresses)
                    {
                        _ = result.Append($"{item.Key}");
                        for (int j = 0; j < listPing[i].Count; j++)
                        {
                            result.Append($" / {listPing[i][j]}");
                        }
                        _ = result.Append($" / avg {listPing[i].Average()}");
                        _ = result.Append($" / ↓ {listPing[i].Min()}");
                        _ = result.Append($" / ↑ {listPing[i].Max()}");
                        _ = result.Append($" / stdev {Math.Round(Math.Sqrt(listPing[i].Average(x => x * x) - Math.Pow(listPing[i].Average(), 2)), 2)}{Environment.NewLine}");
                        i++;
                    }
                    window.Close();
                    AsyncDialog.Open(result.ToString(), "Re: TTSCat");
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch (Exception ex)
            {
                AsyncDialog.Open("延迟测试错误: " + ex.ToString(), "Re: TTSCat", MessageBoxIcon.Error);
            }
        }

        private async void Button_TestGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await TTSPlayer.UnifiedPlay(TextBox_TTSTest.Text);
            }
            catch (Exception ex)
            {
                AsyncDialog.Open("错误: " + ex.ToString(), "Re: TTSCat", MessageBoxIcon.Error);
            }
        }

        private async void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            await Apply();
            Button_Cancel_Click(null, null);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Button_Reload_Click(object sender, RoutedEventArgs e)
        {
            await OnLoad(null, null);
        }

        private void UpdateStats()
        {
            TextBlock_TTSInQueue.Text = TTSPlayer.readerList.Count.ToString();
            long totalSize = 0;
            int count = 0;
            var frame = new DispatcherFrame();
            var thread = new Thread(() =>
            {
                foreach (FileInfo file in (new DirectoryInfo(Vars.cacheDir)).GetFiles())
                {
                    totalSize += file.Length;
                    count++;
                }
                frame.Continue = false;
            });
            thread.Start();
            Dispatcher.PushFrame(frame);
            TextBlock_CacheSize.Text = (totalSize > 1048576L) ? $"{Math.Round((double)(totalSize / 1048576), 2)} MiB" : $"{Math.Round((double)(totalSize / 1024), 2)} KiB";
            TextBlock_CacheSize.Text += $" / {count} 个文件";
            TextBlock_TotalPlayed.Text = Vars.TotalPlayed.ToString();
        }

        private void UpdateStatsThread()
        {
            
            while (!WindowClosed)
            {
                Dispatcher.Invoke(() => UpdateStats());
                Thread.Sleep(1000);
            }
        }

        private async Task Apply()
        {
            Vars.CurrentConf.AllowConnectEvents = CheckBox_ProcessEvents.IsChecked ?? true;
            Vars.CurrentConf.ClearQueueAfterDisconnect = CheckBox_ClearQueueOnDisconnect.IsChecked ?? true;
            Vars.CurrentConf.AllowDownloadMessage = CheckBox_AllowDownloadMessage.IsChecked ?? true;
            Vars.CurrentConf.AutoUpdate = CheckBox_AutoUpdates.IsChecked ?? false;
            Vars.CurrentConf.DebugMode = CheckBox_DebugMode.IsChecked ?? false;
            Vars.CurrentConf.DoNotKeepCache = CheckBox_DoNotKeepCache.IsChecked ?? false;
            Vars.CurrentConf.ReadInQueue = CheckBox_ReadInQueue.IsChecked ?? false;
            Vars.CurrentConf.HttpAuth = CheckBox_EnableHTTPAuth.IsChecked ?? false;
            Vars.CurrentConf.MinimumDanmakuLength = (int)Math.Round(Slider_DMLengthLimit.Value);
            Vars.CurrentConf.MaximumDanmakuLength = (int)Math.Round(Slider_DMLengthLimitMax.Value);
            Vars.CurrentConf.ReadPossibility = (int)Math.Round(Slider_ReadPossibility.Value);
            Vars.CurrentConf.DownloadFailRetryCount = (byte)Math.Round(Slider_RetryCount.Value);
            Vars.CurrentConf.TTSVolume = (int)Math.Round(Slider_TTSVolume.Value);
            Vars.CurrentConf.ReadSpeed = (int)Math.Round(Slider_TTSSpeed.Value);
            Vars.CurrentConf.CustomEngineURL = TextBox_CustomEngineURL.Text;
            Vars.CurrentConf.HttpAuthUsername = TextBox_HTTPAuthUsername.Text;
            Vars.CurrentConf.HttpAuthPassword = TextBox_HTTPAuthPassword.Password;
            Vars.CurrentConf.Engine = (byte)ComboBox_Engine.SelectedIndex;
            Vars.CurrentConf.BlockMode = (byte)ComboBox_Blockmode.SelectedIndex;
            Vars.CurrentConf.GiftBlockMode = (byte)ComboBox_GiftBlockMode.SelectedIndex;
            Vars.CurrentConf.KeywordBlockMode = (byte)ComboBox_KeywordBlockMode.SelectedIndex;
            Vars.CurrentConf.BlackList = TextBox_Blacklist.Text;
            Vars.CurrentConf.WhiteList = TextBox_Whitelist.Text;
            Vars.CurrentConf.GiftBlackList = TextBox_GiftBlacklist.Text;
            Vars.CurrentConf.GiftWhiteList = TextBox_GiftWhitelist.Text;
            Vars.CurrentConf.KeywordBlackList = TextBox_KeywordBlacklist.Text;
            Vars.CurrentConf.KeywordWhiteList = TextBox_KeywordWhitelist.Text;
            UpdateSliders(null, null);
            Vars.CurrentConf.OnGuardBuy = TextBox_GuardBuy.Text;
            Vars.CurrentConf.OnLiveEnd = TextBox_LiveEnd.Text;
            Vars.CurrentConf.OnLiveStart = TextBox_LiveStart.Text;
            Vars.CurrentConf.OnDanmaku = TextBox_OnDanmaku.Text;
            Vars.CurrentConf.OnGift = TextBox_OnGift.Text;
            Vars.CurrentConf.OnWelcome = TextBox_Welcome.Text;
            Vars.CurrentConf.OnWelcomeGuard = TextBox_WelcomeGuard.Text;
            await Conf.SaveAsync();
            await OnLoad(null, null);
        }

        private void Load()
        {
            CheckBox_AutoUpdates.IsChecked = Vars.CurrentConf.AutoUpdate;
            CheckBox_DebugMode.IsChecked = Vars.CurrentConf.DebugMode;
            CheckBox_DoNotKeepCache.IsChecked = Vars.CurrentConf.DoNotKeepCache;
            CheckBox_ReadInQueue.IsChecked = Vars.CurrentConf.ReadInQueue;
            CheckBox_ProcessEvents.IsChecked = Vars.CurrentConf.AllowConnectEvents;
            CheckBox_ClearQueueOnDisconnect.IsChecked = Vars.CurrentConf.ClearQueueAfterDisconnect;
            CheckBox_EnableHTTPAuth.IsChecked = Vars.CurrentConf.HttpAuth;
            CheckBox_AllowDownloadMessage.IsChecked = Vars.CurrentConf.AllowDownloadMessage;
            CheckBox_IsPluginActive.IsChecked = Main.IsEnabled;
            Slider_DMLengthLimit.Value = Vars.CurrentConf.MinimumDanmakuLength;
            Slider_DMLengthLimitMax.Value = Vars.CurrentConf.MaximumDanmakuLength;
            Slider_ReadPossibility.Value = Vars.CurrentConf.ReadPossibility;
            Slider_RetryCount.Value = Vars.CurrentConf.DownloadFailRetryCount;
            Slider_TTSVolume.Value = Vars.CurrentConf.TTSVolume;
            Slider_TTSSpeed.Value = Vars.CurrentConf.ReadSpeed;
            TextBox_CustomEngineURL.Text = Vars.CurrentConf.CustomEngineURL;
            TextBox_HTTPAuthUsername.Text = Vars.CurrentConf.HttpAuthUsername;
            TextBox_HTTPAuthPassword.Password = Vars.CurrentConf.HttpAuthPassword;
            ComboBox_Engine.SelectedIndex = Vars.CurrentConf.Engine;
            ComboBox_Blockmode.SelectedIndex = Vars.CurrentConf.BlockMode;
            ComboBox_GiftBlockMode.SelectedIndex = Vars.CurrentConf.GiftBlockMode;
            ComboBox_KeywordBlockMode.SelectedIndex = Vars.CurrentConf.KeywordBlockMode;
            TextBox_Blacklist.Text = Vars.CurrentConf.BlackList;
            TextBox_Whitelist.Text = Vars.CurrentConf.WhiteList;
            TextBox_GiftBlacklist.Text = Vars.CurrentConf.GiftBlackList;
            TextBox_GiftWhitelist.Text = Vars.CurrentConf.GiftWhiteList;
            TextBox_KeywordBlacklist.Text = Vars.CurrentConf.KeywordBlackList;
            TextBox_KeywordWhitelist.Text = Vars.CurrentConf.KeywordWhiteList;
            UpdateSliders(null, null);
            TextBox_GuardBuy.Text = Vars.CurrentConf.OnGuardBuy;
            TextBox_LiveEnd.Text = Vars.CurrentConf.OnLiveEnd;
            TextBox_LiveStart.Text = Vars.CurrentConf.OnLiveStart;
            TextBox_OnDanmaku.Text = Vars.CurrentConf.OnDanmaku;
            TextBox_OnGift.Text = Vars.CurrentConf.OnGift;
            TextBox_Welcome.Text = Vars.CurrentConf.OnWelcome;
            TextBox_WelcomeGuard.Text = Vars.CurrentConf.OnWelcomeGuard;

            TextBox_Debug.Clear();
            TextBox_Debug.AppendText("---------- OS Environment ----------\n");
            TextBox_Debug.AppendText($"Operating system: {Environment.OSVersion.ToString()}\n");
            TextBox_Debug.AppendText("---------- Plugin Environment ----------\n");
            TextBox_Debug.AppendText($"Plugin version: {Vars.currentVersion.ToString()}\n");
            TextBox_Debug.AppendText($"Plugin executable: {Vars.dllFileName}\n");
            TextBox_Debug.AppendText($"Plugin configuration directory: {Vars.confDir}\n");
            TextBox_Debug.AppendText($"Audio library file: {Vars.audioLibFileName}\n");
            TextBox_Debug.AppendText($"Plugins directory: {Vars.dllPath}\n");
            if (Vars.CurrentConf.DebugMode)
            {
                try
                {
                    TextBox_Debug.AppendText("---------- [DEBUG MODE ACTIVE, ADVANCED INFO VISIBLE] ----------\n");
                    TextBox_Debug.AppendText("---------- Advanced OS Environment ----------\n");
                    var wmi = new ManagementObjectSearcher("select * from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
                    TextBox_Debug.AppendText("(OS info retrieved via WMI)\n");
                    TextBox_Debug.AppendText($"OS name: {((string)wmi["Caption"]).Trim()}\n");
                    TextBox_Debug.AppendText($"OS version: {(string)wmi["Version"]}\n");
                    TextBox_Debug.AppendText($"Max processes: {(uint)wmi["MaxNumberOfProcesses"]}\n");
                    TextBox_Debug.AppendText($"Max process RAM usage: {(ulong)wmi["MaxProcessMemorySize"] / 1024 / 1024} MiB\n");
                    TextBox_Debug.AppendText($"Architecture: {(string)wmi["OSArchitecture"]}\n");
                    TextBox_Debug.AppendText($"Serial number: {(string)wmi["SerialNumber"]}\n");
                    TextBox_Debug.AppendText($"Build number: {((string)wmi["BuildNumber"])}\n");
                    TextBox_Debug.AppendText($"Security manager: {(SystemInformation.Secure ? "present" : "not present")}\n");
                    TextBox_Debug.AppendText($"Network: {(SystemInformation.Network ? "yes" : "no")}\n");
                    TextBox_Debug.AppendText($"Boot mode: {(SystemInformation.BootMode == BootMode.Normal ? "normal" : "safe")}\n");

                    TextBox_Debug.AppendText("---------- .NET Environment ----------\n");
                    TextBox_Debug.AppendText($"CLR: {Environment.Version.ToString()}\n");
                    var assembliesArray = AppDomain.CurrentDomain.GetAssemblies();
                    var assemblies = assembliesArray.ToList();
                    assemblies.Sort(new AssemblyComparer());
                    TextBox_Debug.AppendText($"Loaded assemblies ({assemblies.Count}): \n");
                    foreach (var assembly in assemblies)
                    {
                        TextBox_Debug.AppendText($"{assembly.FullName}{(!assembly.IsDynamic ? $"@{assembly.Location}" : "")}\n");
                    }
                }
                catch (Exception ex)
                {
                    TextBox_Debug.AppendText($"Error retrieving advanced log: {ex.ToString()}\n");
                }
            }
            UpdateStats();

            TabItem_DebugOptions.Visibility = Vars.CurrentConf.DebugMode ? Visibility.Visible : Visibility.Hidden;
            this.Title = $"{Vars.mgmtWindowTitle}{(Vars.CurrentConf.DebugMode ? " *用户调试模式*" : "")}{(Debugger.IsAttached ? " *弱智模式*" : "")}";
        }

        private async void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            await Apply();
        }

        private void Button_Donate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://daily.elepover.com/donate/");
        }

        private void Button_CheckUpd_Click(object sender, RoutedEventArgs e)
        {
            updateWindow = new UpdateWindow();
            updateWindow.ShowDialog();
        }

        private void UpdateSliders(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_updateSliderAllowed) { return; }
            TextBlock_DMLengthLimit.Text = Math.Round(Slider_DMLengthLimit.Value).ToString();
            TextBlock_DMLengthLimitMax.Text = Math.Round(Slider_DMLengthLimitMax.Value).ToString();
            TextBlock_ReadPossibility.Text = Math.Round(Slider_ReadPossibility.Value).ToString();
            TextBlock_RetryCount.Text = Math.Round(Slider_RetryCount.Value).ToString();
            TextBlock_TTSVolume.Text = Math.Round(Slider_TTSVolume.Value).ToString();
            TextBlock_TTSSpeed.Text = Math.Round(Slider_TTSSpeed.Value).ToString();
            Slider_DMLengthLimit.Maximum = Slider_DMLengthLimitMax.Value;
            Slider_DMLengthLimitMax.Minimum = Slider_DMLengthLimit.Value;
        }
        
        private async Task OnLoad(object sender, RoutedEventArgs e)
        {
            await Conf.InitiateAsync();
            Load();
        }

        private void Button_About_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.danmuji.org/plugins/Re-TTSCat");
        }

        private void Button_Suggestions_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://daily.elepover.com/comments/");
        }

        bool _shown = false;
        bool _updateSliderAllowed = false;

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;
            WindowClosed = false;

            await OnLoad(null, null);
            _updateSliderAllowed = true;
            UpdateSliders(null, null);
            // update information in the thanks field
            // THIS IS NOT TELEMETRY, WE RESPECT OUR USERS' PRIVACY AND WILL NEVER DO SO
            if (Vars.CurrentConf.AllowDownloadMessage)
            {
                var downloader = new Thread(() =>
                {
                    string str = "感谢使用本插件", comments = "Copyright (C) 2017 - 2020 Elepover.\nThis is an open-source(MIT) software.";
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.Headers.Set(HttpRequestHeader.Referer, "https://www.danmuji.org/plugins/Re-TTSCat");
                            client.Headers.Set(HttpRequestHeader.UserAgent, $"Re_TTSCat/{Vars.currentVersion.ToString()} (Windows NT {Environment.OSVersion.Version.ToString(2)}; {(Environment.Is64BitOperatingSystem ? "Win64; x64" : "Win32; x86")})");
                            str = Encoding.UTF8.GetString(client.DownloadData("https://static-cn.itsmy.app:12306/files/today"));
                            comments = Encoding.UTF8.GetString(client.DownloadData("https://static-cn.itsmy.app:12306/files/today_comments"));
                        }
                        catch { }
                    }
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TextBlock_Thanks.Text = str;
                            TextBlock_Thanks.ToolTip = comments;
                        });
                    }
                    catch { }
                })
                {
                    IsBackground = true
                };
                downloader.Start();
            }
        }

        private void Button_DeleteCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (FileInfo fileInfo in (new DirectoryInfo(Vars.cacheDir)).GetFiles())
                {
                    fileInfo.Delete();
                }
                AsyncDialog.Open("OK");
            }
            catch (Exception ex)
            {
                AsyncDialog.Open("Error: " + ex.ToString(), "Re: TTSCat", MessageBoxIcon.Error);
            }
        }

        private void Button_ManuallyCrash_Click(object sender, RoutedEventArgs e)
        {
            throw new NullReferenceException();
        }

        private void Button_StartDebugger_Click(object sender, RoutedEventArgs e)
        {
            Debugger.Launch();
        }

        private void Button_PlayAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var frame = new DispatcherFrame();
                var thread = new Thread(() => {
                    var waveOut = new WaveOutEvent();
                    var reader = new AudioFileReader(Interaction.InputBox("输入文件名", "Re: TTSCat"));
                    waveOut.Init(reader);
                    waveOut.Play();
                    frame.Continue = false;
                });
                thread.Start();
                Dispatcher.PushFrame(frame);
            }
            catch (Exception ex)
            {
                AsyncDialog.Open("Error: " + ex.ToString(), "Re: TTSCat", MessageBoxIcon.Error);
            }
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.Windows.MessageBox.Show("您确定要重置所有设置到默认值吗？", "Re: TTSCat", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Vars.CurrentConf = new Conf();
                    Load();
                    System.Windows.MessageBox.Show("已恢复所有设置至默认值，点击保存或应用以保存到配置文件中。", "Re: TTSCat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("配置重置失败: " + ex.Message, "Re: TTSCat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_PermissionInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("系统权限信息\n\nRe: TTSCat 使用了以下系统权限:\n\n• 读取/写入您的文件系统\n插件需要该权限以进行配置读取和保存及 TTS 文件生成与读取\n\n• 访问互联网\n用于下载 TTS 文件及检查更新\n\n• 获取并控制音频设备\n用于播放语音", "Re: TTSCat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CheckBox_AutoUpdate_Checked(object sender, RoutedEventArgs e)
        {
            if ((StatsUpdater == null) || !StatsUpdater.IsAlive)
            {
                StatsUpdater = new Thread(() => UpdateStatsThread());
            }
            StatsUpdater.Start();
        }

        private void CheckBox_AutoUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            StatsUpdater.Abort();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // close all subwindows
            if ((updateWindow != null) && (updateWindow.IsOpen)) updateWindow.Close();
            WindowClosed = true;
            WindowDisposed = true;
        }

        private void CheckBox_ProcessEvents_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!_updateSliderAllowed) return;
            if (CheckBox_ProcessEvents.IsChecked ?? true)
            {
                CheckBox_ClearQueueOnDisconnect.IsEnabled = true;
            }
            else
            {
                CheckBox_ClearQueueOnDisconnect.IsEnabled = false;
                CheckBox_ClearQueueOnDisconnect.IsChecked = false;
            }
        }

        private void Button_ClearQueue_Click(object sender, RoutedEventArgs e)
        {
            TTSPlayer.readerList.Clear();
        }

        private void Button_ClearCache_Click(object sender, RoutedEventArgs e)
        {
            if (TTSPlayer.readerList.Count > 0)
            {
                AsyncDialog.Open($"还有 {TTSPlayer.readerList.Count} 个语音在队列中，删除可能导致插件工作异常\n\n请在播放完毕后再试一次", icon: MessageBoxIcon.Warning);
                return;
            }
            try
            {
                long totalSize = 0;
                int count = 0;
                foreach (FileInfo fileInfo in (new DirectoryInfo(Vars.cacheDir)).GetFiles())
                {
                    totalSize += fileInfo.Length;
                    fileInfo.Delete();
                    count++;
                }
                if (count == 0)
                {
                    AsyncDialog.Open("无缓存文件，无需删除", icon: MessageBoxIcon.Information);
                }
                else
                {
                    AsyncDialog.Open($"成功！\n\n已删除 {count} 个文件，共计释放 {((totalSize > 1048576L) ? $"{Math.Round((double)(totalSize / 1048576), 2)} MiB" : $"{Math.Round((double)(totalSize / 1024), 2)} KiB")} 的存储空间", icon: MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                AsyncDialog.Open($"出错: {ex.ToString()}", icon: MessageBoxIcon.Error);
            }
        }

        private void CheckBox_EnableHTTPAuth_Checked(object sender, RoutedEventArgs e)
        {
            TextBox_HTTPAuthUsername.IsEnabled = true;
            TextBox_HTTPAuthPassword.IsEnabled = true;
        }

        private void CheckBox_EnableHTTPAuth_Unchecked(object sender, RoutedEventArgs e)
        {
            TextBox_HTTPAuthUsername.IsEnabled = false;
            TextBox_HTTPAuthPassword.IsEnabled = false;
        }
    }
}
