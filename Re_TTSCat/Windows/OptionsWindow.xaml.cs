using BilibiliDM_PluginFramework;
using Microsoft.VisualBasic; // ← 不愧是我
using NAudio.Wave;
using Newtonsoft.Json;
using Re_TTSCat.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Principal;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

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
            DataContext = this;
            ListView_VoiceReplyRules.ItemsSource = voiceReplyRulesDataSource;
            voiceReplyRulesDataSource.CollectionChanged += VoiceReplyRulesDataSource_CollectionChanged;
        }

        private void VoiceReplyRulesDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListView_VoiceReplyRules.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty)?.UpdateTarget();
        }

        public bool WindowDisposed = false;
        private bool windowClosed = true;
        private DonateWindow donateWindow = new DonateWindow();
        private UpdateWindow updateWindow = new UpdateWindow();
        private Thread statsUpdater;
        private ObservableCollection<VoiceReplyRule> voiceReplyRulesDataSource = new ObservableCollection<VoiceReplyRule>();

        private async Task DarkenAsync()
        {
            WebBrowser_Today.Visibility = Visibility.Hidden; // sry but we have to do this
            Grid_AnimationContainer.Visibility = Visibility.Visible;

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 5,
                Duration = new TimeSpan(0, 0, 0, 0, 625)
            };
            animation.EasingFunction = new PowerEase() { Power = 15, EasingMode = EasingMode.EaseOut };

            var effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = 0 };
            Grid_Master.Effect = effect;
            Grid_Master.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);

            var sb = Grid_AnimationContainer.FindResource("DarkenAnimation") as Storyboard;
            await sb.BeginAsync();
        }

        private async Task BrightenAsync()
        {
            if (Grid_Master.Effect != null)
            {
                var animation = new DoubleAnimation
                {
                    From = 5,
                    To = 0,
                    Duration = new TimeSpan(0, 0, 0, 0, 625)
                };
                animation.EasingFunction = new PowerEase() { Power = 15, EasingMode = EasingMode.EaseIn };

                Grid_Master.Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            }

            var sb = Grid_AnimationContainer.FindResource("BrightenAnimation") as Storyboard;
            await sb.BeginAsync();
            Grid_Master.Effect = null;
            Grid_AnimationContainer.Visibility = Visibility.Hidden;
            if (Vars.CurrentConf.AllowDownloadMessage)
                WebBrowser_Today.Visibility = Visibility.Visible;
        }

        private async void Button_CheckConnectivity_Click(object sender, RoutedEventArgs e)
        {
            await DarkenAsync();
            TextBox_ExecutionResult.Text = "延迟测试已启动...";
            try
            {
                var thread = new Thread(() =>
                {
                    var window = new LoadingWindowLight();
                    window.IsOpen = true;
                    var result = new StringBuilder();
                    var ip = new WebClient().DownloadString("https://apis.elepover.com/ip/");
                    result.Append($"设备 IP（公网）: {ip}{Environment.NewLine}");
                    var listPing = new List<List<long>>();
                    var listAddresses = new Dictionary<string, string>
                    {
                        { "Google", "https://translate.google.cn/" },
                        { "百度", "https://fanyi.baidu.com/" },
                        { "百度 (TSN)", "https://tsn.baidu.com/" },
                        { "有道", "http://tts.youdao.com/" }
                    };
                    var sw = new Stopwatch();
                    int i = 0;
                    foreach (var item in listAddresses)
                    {
                        window.ProgressBar.Value = (double)(i * 100) / listAddresses.Count;
                        var list = new List<long>();
                        for (int j = 0; j < 11; j++)
                        {
                            var req = WebRequest.CreateHttp(item.Value);
                            req.Method = "HEAD";
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
                            if (j != 0) // ditch first initial connection (stupid.png)
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
                    window.IsOpen = false;
                    AsyncDialog.Open(result.ToString(), "Re: TTSCat");
                    Dispatcher.Invoke(async () =>
                    {
                        TextBox_ExecutionResult.Text = "延迟测试完成";
                        await BrightenAsync();
                    });
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch (Exception ex)
            {
                AsyncDialog.Open($"延迟测试错误: {ex}", "Re: TTSCat", MessageBoxIcon.Error);
                TextBox_ExecutionResult.Text = $"延迟测试错误: {ex.Message}";
            }
        }

        private async void Button_TestGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await TTSPlayer.UnifiedPlay(TextBox_TTSTest.Text);
                TextBox_ExecutionResult.Text = "播放成功/已添加到队列";
            }
            catch (Exception ex)
            {
                AsyncDialog.Open($"错误: {ex}", "Re: TTSCat", MessageBoxIcon.Error);
                TextBox_ExecutionResult.Text = $"播放失败: {ex.Message}";
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
            await DarkenAsync();
            await OnLoad(null, null);
            await BrightenAsync();
        }

        private void UpdateStats()
        {
            TextBlock_TTSInQueue.Text = TTSPlayer.fileList.Count.ToString();
            long totalSize = 0;
            int count = 0;
            var frame = new DispatcherFrame();
            var thread = new Thread(() =>
            {
                foreach (var file in (new DirectoryInfo(Vars.DefaultCacheDir)).GetFiles())
                {
                    totalSize += file.Length;
                    count++;
                }
                foreach (var file in (new DirectoryInfo(Vars.CacheDirTemp)).GetFiles())
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
            TextBlock_TotalFailed.Text = Vars.TotalFails.ToString();
        }

        private void UpdateStatsThread()
        {
            
            while (!windowClosed)
            {
                Dispatcher.Invoke(() => UpdateStats());
                Thread.Sleep(1000);
            }
        }

        private async Task Apply()
        {
            Vars.CurrentConf.AllowConnectEvents = CheckBox_ProcessEvents.IsChecked ?? true;
            Vars.CurrentConf.CatchGlobalError = CheckBox_CatchGlobalError.IsChecked ?? true;
            Vars.CurrentConf.ClearQueueAfterDisconnect = CheckBox_ClearQueueOnDisconnect.IsChecked ?? true;
            Vars.CurrentConf.AllowDownloadMessage = CheckBox_AllowDownloadMessage.IsChecked ?? true;
            Vars.CurrentConf.AutoUpdate = CheckBox_AutoUpdates.IsChecked ?? false;
            Vars.CurrentConf.DebugMode = CheckBox_DebugMode.IsChecked ?? false;
            Vars.CurrentConf.DoNotKeepCache = CheckBox_DoNotKeepCache.IsChecked ?? false;
            Vars.CurrentConf.SaveCacheInTempDir = CheckBox_SaveCacheInTemp.IsChecked ?? true;
            Vars.CurrentConf.ReadInQueue = CheckBox_ReadInQueue.IsChecked ?? false;
            Vars.CurrentConf.SuperChatIgnoreRandomDitch = CheckBox_SuperChatIgnoreRandomDitch.IsChecked ?? true;
            Vars.CurrentConf.HttpAuth = CheckBox_EnableHTTPAuth.IsChecked ?? false;
            Vars.CurrentConf.SuppressLogOutput = CheckBox_SuppressLogOutput.IsChecked ?? false;
            Vars.CurrentConf.OverrideToLogsTabOnStartup = CheckBox_OverrideToLogsTabOnStartup.IsChecked ?? false;
            Vars.CurrentConf.AutoStartOnLoad = CheckBox_AutoStartOnLoad.IsChecked ?? false;
            Vars.CurrentConf.ClearCacheOnStartup = CheckBox_ClearCacheOnStartup.IsChecked ?? true;
            Vars.CurrentConf.SpeechPerson = ComboBox_Person.SelectedIndex;
            Vars.CurrentConf.DeviceGuid = ((PlaybackDeviceWrapper)ComboBox_OutputDevice.SelectedItem).DeviceGuid;
            var tempVoiceName = (string)ComboBox_VoiceName.SelectedItem ?? "(无效选择)";
            if ((tempVoiceName != Vars.CurrentConf.VoiceName) && Vars.SystemSpeechAvailable)
            {
                var warningMessage = new StringBuilder($"选择的语音包 \"{tempVoiceName}\" 存在问题，可能无法正常使用：{Environment.NewLine}");
                var voiceFound = false;
                var warningTriggered = false;
                using (var synth = new SpeechSynthesizer())
                {
                    foreach (var voice in synth.GetInstalledVoices())
                    {
                        if (voice.VoiceInfo.Name == tempVoiceName)
                        {
                            voiceFound = true;
                            if (!voice.Enabled)
                            {
                                warningTriggered = true;
                                warningMessage.Append(Environment.NewLine);
                                warningMessage.Append("语音包已安装，但未启用");
                            }
                            if (voice.VoiceInfo.Culture.TwoLetterISOLanguageName.ToLowerInvariant() != "zh")
                            {
                                warningTriggered = true;
                                warningMessage.Append(Environment.NewLine);
                                warningMessage.Append($"语音包目标语言 \"{voice.VoiceInfo.Culture.DisplayName}\" 非中文");
                            }
                            break;
                        }
                    }
                    if (!voiceFound)
                    {
                        warningTriggered = true;
                        warningMessage.Append(Environment.NewLine);
                        warningMessage.Append("系统上并未找到此语音包");
                    }
                }
                if (warningTriggered)
                {
                    warningMessage.Append(Environment.NewLine);
                    warningMessage.Append(Environment.NewLine);
                    warningMessage.Append("是否仍要保存语音包选择？");
                    if (System.Windows.Forms.MessageBox.Show(warningMessage.ToString(), "语音包警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Vars.CurrentConf.VoiceName = tempVoiceName;
                    }
                }
                else Vars.CurrentConf.VoiceName = tempVoiceName;
            }
            Vars.CurrentConf.EnableVoiceReply = CheckBox_EnableVoiceReply.IsChecked ?? false;
            Vars.CurrentConf.InstantVoiceReply = CheckBox_InstantVoiceReply.IsChecked ?? false;
            Vars.CurrentConf.MinifyJson = CheckBox_MinifyJson.IsChecked ?? true;
            Vars.CurrentConf.GiftsThrottle = CheckBox_GiftsThrottle.IsChecked ?? true;
            Vars.CurrentConf.EnableUrlEncode = CheckBox_UrlEncode.IsChecked ?? true;
            Vars.CurrentConf.VoiceReplyFirst = CheckBox_VoiceReplyFirst.IsChecked ?? false;
            Vars.CurrentConf.IgnoreIfHitVoiceReply = CheckBox_IgnoreIfHit.IsChecked ?? false;
            Vars.CurrentConf.AutoFallback = CheckBox_AutoFallback.IsChecked ?? true;
            Vars.CurrentConf.UseDirectSound = CheckBox_UseDirectSound.IsChecked ?? true;
            Vars.CurrentConf.AutoBaiduFallback = CheckBox_AutoBaiduFallback.IsChecked ?? true;
            Vars.CurrentConf.BlockUID = ComboBox_BlockType.SelectedIndex == 0;
            Vars.CurrentConf.MinimumDanmakuLength = (int)Math.Round(Slider_DMLengthLimit.Value);
            Vars.CurrentConf.MaximumDanmakuLength = (int)Math.Round(Slider_DMLengthLimitMax.Value);
            Vars.CurrentConf.ReadPossibility = (int)Math.Round(Slider_ReadPossibility.Value);
            Vars.CurrentConf.DownloadFailRetryCount = (byte)Math.Round(Slider_RetryCount.Value);
            Vars.CurrentConf.TTSVolume = (int)Math.Round(Slider_TTSVolume.Value);
            Vars.CurrentConf.ReadSpeed = (int)Math.Round(Slider_TTSSpeed.Value);
            Vars.CurrentConf.SpeechPitch = (int)Math.Round(Slider_TTSPitch.Value);
            Vars.CurrentConf.GiftsThrottleDuration = (int)Math.Round(Slider_GiftThrottleDuration.Value);
            Vars.CurrentConf.CustomEngineURL = TextBox_CustomEngineURL.Text;
            Vars.CurrentConf.HttpAuthUsername = TextBox_HTTPAuthUsername.Text;
            Vars.CurrentConf.HttpAuthPassword = TextBox_HTTPAuthPassword.Password;
            Vars.CurrentConf.Engine = (byte)ComboBox_Engine.SelectedIndex;
            Vars.CurrentConf.ReqType = (RequestType)ComboBox_PostMethod.SelectedIndex;
            Vars.CurrentConf.BlockMode = (byte)ComboBox_Blockmode.SelectedIndex;
            Vars.CurrentConf.GiftBlockMode = (byte)ComboBox_GiftBlockMode.SelectedIndex;
            Vars.CurrentConf.KeywordBlockMode = (byte)ComboBox_KeywordBlockMode.SelectedIndex;
            Vars.CurrentConf.PostData = TextBox_PostData.Text;
            Vars.CurrentConf.BaiduApiKey = TextBox_BaiduApiKey.Text;
            Vars.CurrentConf.BaiduApiSecretKey = TextBox_BaiduApiSecretKey.Password;
            try
            {
                Vars.CurrentConf.Headers = JsonConvert.DeserializeObject<List<Header>>(TextBox_Headers.Text);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"未保存请求头数据: 无法解析 JSON: {ex.Message}\n其他数据均将继续尝试正常保存", "Re: TTSCat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            Vars.CurrentConf.OnSuperChat = TextBox_OnSuperChat.Text;
            Vars.CurrentConf.OnGift = TextBox_OnGift.Text;
            Vars.CurrentConf.OnWelcome = TextBox_Welcome.Text;
            Vars.CurrentConf.OnWelcomeGuard = TextBox_WelcomeGuard.Text;
            Vars.CurrentConf.OnWarning = TextBox_SuperAdminWarnings.Text;
            Vars.CurrentConf.OnInteractEnter = TextBox_InteractEnter.Text;
            Vars.CurrentConf.OnInteractFollow = TextBox_InteractFollow.Text;
            Vars.CurrentConf.OnInteractMutualFollow = TextBox_InteractMutualFollow.Text;
            Vars.CurrentConf.OnInteractShare = TextBox_InteractShare.Text;
            Vars.CurrentConf.OnInteractSpecialFollow = TextBox_InteractSpecialFollow.Text;
            Vars.CurrentConf.VoiceReplyRules = new List<VoiceReplyRule>(voiceReplyRulesDataSource);
            // try to resolve custom titles
            try
            {
                var raw = TextBox_CustomTitles.Text;
                if (raw.Count(x => x == '/') < 4)
                {
                    throw new ArgumentException("参数不足，是否已设定所有值？");
                }
                var array = raw.Split('/');
                Vars.CurrentConf.CustomVIP = array[0];
                Vars.CurrentConf.CustomGuardLevel0 = array[1];
                Vars.CurrentConf.CustomGuardLevel1 = array[2];
                Vars.CurrentConf.CustomGuardLevel2 = array[3];
                Vars.CurrentConf.CustomGuardLevel3 = array[4];
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"未保存自定义头衔数据: 无法解析: {ex.Message}\n其他数据均将继续尝试正常保存", "Re: TTSCat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await Conf.SaveAsync();
            await OnLoad(null, null);
        }

        private void Load()
        {
            CheckBox_AutoUpdates.IsChecked = Vars.CurrentConf.AutoUpdate;
            CheckBox_CatchGlobalError.IsChecked = Vars.CurrentConf.CatchGlobalError;
            CheckBox_DebugMode.IsChecked = Vars.CurrentConf.DebugMode;
            CheckBox_DoNotKeepCache.IsChecked = Vars.CurrentConf.DoNotKeepCache;
            CheckBox_SaveCacheInTemp.IsChecked = Vars.CurrentConf.SaveCacheInTempDir;
            CheckBox_ReadInQueue.IsChecked = Vars.CurrentConf.ReadInQueue;
            CheckBox_ProcessEvents.IsChecked = Vars.CurrentConf.AllowConnectEvents;
            CheckBox_ClearQueueOnDisconnect.IsChecked = Vars.CurrentConf.ClearQueueAfterDisconnect;
            CheckBox_SuperChatIgnoreRandomDitch.IsChecked = Vars.CurrentConf.SuperChatIgnoreRandomDitch;
            CheckBox_EnableHTTPAuth.IsChecked = Vars.CurrentConf.HttpAuth;
            CheckBox_AllowDownloadMessage.IsChecked = Vars.CurrentConf.AllowDownloadMessage;
            CheckBox_IsPluginActive.IsChecked = Main.IsEnabled;
            CheckBox_ClearCacheOnStartup.IsChecked = Vars.CurrentConf.ClearCacheOnStartup;
            CheckBox_SuppressLogOutput.IsChecked = Vars.CurrentConf.SuppressLogOutput;
            CheckBox_OverrideToLogsTabOnStartup.IsChecked = Vars.CurrentConf.OverrideToLogsTabOnStartup;
            CheckBox_AutoStartOnLoad.IsChecked = Vars.CurrentConf.AutoStartOnLoad;
            CheckBox_EnableVoiceReply.IsChecked = Vars.CurrentConf.EnableVoiceReply;
            CheckBox_InstantVoiceReply.IsChecked = Vars.CurrentConf.InstantVoiceReply;
            CheckBox_MinifyJson.IsChecked = Vars.CurrentConf.MinifyJson;
            CheckBox_GiftsThrottle.IsChecked = Vars.CurrentConf.GiftsThrottle;
            CheckBox_UrlEncode.IsChecked = Vars.CurrentConf.EnableUrlEncode;
            CheckBox_VoiceReplyFirst.IsChecked = Vars.CurrentConf.VoiceReplyFirst;
            CheckBox_IgnoreIfHit.IsChecked = Vars.CurrentConf.IgnoreIfHitVoiceReply;
            CheckBox_AutoFallback.IsChecked = Vars.CurrentConf.AutoFallback;
            CheckBox_UseDirectSound.IsChecked = Vars.CurrentConf.UseDirectSound;
            CheckBox_AutoBaiduFallback.IsChecked = Vars.CurrentConf.AutoBaiduFallback;
            Slider_DMLengthLimit.Value = Vars.CurrentConf.MinimumDanmakuLength;
            Slider_DMLengthLimitMax.Value = Vars.CurrentConf.MaximumDanmakuLength;
            Slider_ReadPossibility.Value = Vars.CurrentConf.ReadPossibility;
            Slider_RetryCount.Value = Vars.CurrentConf.DownloadFailRetryCount;
            Slider_TTSVolume.Value = Vars.CurrentConf.TTSVolume;
            Slider_TTSSpeed.Value = Vars.CurrentConf.ReadSpeed;
            Slider_TTSPitch.Value = Vars.CurrentConf.SpeechPitch;
            Slider_GiftThrottleDuration.Value = Vars.CurrentConf.GiftsThrottleDuration;
            TextBox_CustomEngineURL.Text = Vars.CurrentConf.CustomEngineURL;
            TextBox_HTTPAuthUsername.Text = Vars.CurrentConf.HttpAuthUsername;
            TextBox_HTTPAuthPassword.Password = Vars.CurrentConf.HttpAuthPassword;
            TextBox_BaiduApiKey.Text = Vars.CurrentConf.BaiduApiKey;
            TextBox_BaiduApiSecretKey.Password = Vars.CurrentConf.BaiduApiSecretKey;
            ComboBox_Engine.SelectedIndex = Vars.CurrentConf.Engine;
            ComboBox_Person.SelectedIndex = Vars.CurrentConf.SpeechPerson;
            ComboBox_PostMethod.SelectedIndex = (int)Vars.CurrentConf.ReqType;
            ComboBox_Blockmode.SelectedIndex = Vars.CurrentConf.BlockMode;
            ComboBox_GiftBlockMode.SelectedIndex = Vars.CurrentConf.GiftBlockMode;
            ComboBox_KeywordBlockMode.SelectedIndex = Vars.CurrentConf.KeywordBlockMode;
            ComboBox_BlockType.SelectedIndex = Vars.CurrentConf.BlockUID ? 0 : 1;
            var deviceList = new List<PlaybackDeviceWrapper>();
            var matchIndex = 0;
            foreach (var dev in DirectSoundOut.Devices)
            {
                deviceList.Add(new PlaybackDeviceWrapper() { DeviceGuid = dev.Guid });
                if (dev.Guid == Vars.CurrentConf.DeviceGuid) matchIndex = deviceList.Count - 1;
            }
            ComboBox_OutputDevice.ItemsSource = deviceList;
            ComboBox_OutputDevice.SelectedIndex = matchIndex;
            if (Vars.SystemSpeechAvailable)
            {
                using (var synth = new SpeechSynthesizer())
                {
                    ComboBox_VoiceName.Items.Clear();
                    var voices = synth.GetInstalledVoices();
                    if (voices.Count() == 0)
                    {
                        ComboBox_VoiceName.Items.Add(Vars.CurrentConf.VoiceName);
                        ComboBox_VoiceName.SelectedIndex = 0;
                    }
                    else
                    {
                        var voiceMatchIndex = 0;
                        foreach (var voice in voices)
                        {
                            ComboBox_VoiceName.Items.Add(voice.VoiceInfo.Name);
                            if (voice.VoiceInfo.Name == Vars.CurrentConf.VoiceName) voiceMatchIndex = ComboBox_VoiceName.Items.Count - 1;
                        }
                        ComboBox_VoiceName.SelectedIndex = voiceMatchIndex;
                    }
                }
            }
            else ComboBox_VoiceName.IsEnabled = false;
            TextBox_PostData.Text = Vars.CurrentConf.PostData;
            TextBox_Headers.Text = JsonConvert.SerializeObject(Vars.CurrentConf.Headers, Formatting.Indented);
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
            TextBox_OnSuperChat.Text = Vars.CurrentConf.OnSuperChat;
            TextBox_OnGift.Text = Vars.CurrentConf.OnGift;
            TextBox_InteractEnter.Text = Vars.CurrentConf.OnInteractEnter;
            TextBox_InteractFollow.Text = Vars.CurrentConf.OnInteractFollow;
            TextBox_InteractMutualFollow.Text = Vars.CurrentConf.OnInteractMutualFollow;
            TextBox_InteractShare.Text = Vars.CurrentConf.OnInteractShare;
            TextBox_InteractSpecialFollow.Text = Vars.CurrentConf.OnInteractSpecialFollow;
            TextBox_Welcome.Text = Vars.CurrentConf.OnWelcome;
            TextBox_WelcomeGuard.Text = Vars.CurrentConf.OnWelcomeGuard;
            TextBox_CustomTitles.Text = $"{Vars.CurrentConf.CustomVIP}/{Vars.CurrentConf.CustomGuardLevel0}/{Vars.CurrentConf.CustomGuardLevel1}/{Vars.CurrentConf.CustomGuardLevel2}/{Vars.CurrentConf.CustomGuardLevel3}";
            TextBox_SuperAdminWarnings.Text = Vars.CurrentConf.OnWarning;
            Label_AboutTitle.Text = $"Re: TTSCat - Elliptical ({Vars.CurrentVersion})";
            voiceReplyRulesDataSource.Clear();
            foreach (var item in Vars.CurrentConf.VoiceReplyRules)
                voiceReplyRulesDataSource.Add(item);

            TextBox_Debug.Clear();
            TextBox_Debug.AppendText("---------- OS Environment ----------\n");
            TextBox_Debug.AppendText($"Operating system: {Environment.OSVersion}\n");
            TextBox_Debug.AppendText("---------- Plugin Environment ----------\n");
            TextBox_Debug.AppendText($"Plugin version: {Vars.CurrentVersion}\n");
            TextBox_Debug.AppendText($"Plugin executable: {Vars.AppDllFileName}\n");
            TextBox_Debug.AppendText($"Plugin configuration directory: {Vars.ConfDir}\n");
            TextBox_Debug.AppendText($"Cache directory: {Vars.CacheDir}\n");
            TextBox_Debug.AppendText($"Audio library file: {Vars.AudioLibraryFileName}\n");
            TextBox_Debug.AppendText($"Plugins directory: {Vars.AppDllFilePath}\n");
            if (Vars.CurrentConf.DebugMode)
            {
                TextBox_Debug.Visibility = Visibility.Visible;
                try
                {
                    TextBox_Debug.AppendText("---------- [DEBUG MODE ACTIVE, ADVANCED INFO VISIBLE] ----------\n");
                    TextBox_Debug.AppendText($"Running in administrator privileges? {((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator) ? "yes" : "no")}\n");
                    TextBox_Debug.AppendText("---------- Advanced OS Environment ----------\n");
                    var wmi = new ManagementObjectSearcher("select * from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
                    TextBox_Debug.AppendText("(OS info retrieved via WMI)\n");
                    TextBox_Debug.AppendText($"OS name: {((string)wmi["Caption"]).Trim()}\n");
                    TextBox_Debug.AppendText($"OS version: {(string)wmi["Version"]}\n");
                    TextBox_Debug.AppendText($"Max processes: {(uint)wmi["MaxNumberOfProcesses"]}\n");
                    TextBox_Debug.AppendText($"Max process RAM usage: {(ulong)wmi["MaxProcessMemorySize"] / 1024 / 1024} MiB\n");
                    TextBox_Debug.AppendText($"Architecture: {(string)wmi["OSArchitecture"]}\n");
                    TextBox_Debug.AppendText($"Serial number: {(string)wmi["SerialNumber"]}\n");
                    TextBox_Debug.AppendText($"Build number: {(string)wmi["BuildNumber"]}\n");
                    TextBox_Debug.AppendText($"Security manager: {(SystemInformation.Secure ? "present" : "not present")}\n");
                    TextBox_Debug.AppendText($"Network: {(SystemInformation.Network ? "yes" : "no")}\n");
                    TextBox_Debug.AppendText($"Boot mode: {(SystemInformation.BootMode == BootMode.Normal ? "normal" : "safe")}\n");

                    TextBox_Debug.AppendText("---------- .NET Environment ----------\n");
                    TextBox_Debug.AppendText($"CLR: {Environment.Version}\n");
                    var assembliesArray = AppDomain.CurrentDomain.GetAssemblies();
                    var assemblies = assembliesArray.ToList();
                    assemblies.Sort(new AssemblyComparer());
                    TextBox_Debug.AppendText($"Loaded assemblies ({assemblies.Count}): \n");
                    foreach (var assembly in assemblies)
                    {
                        TextBox_Debug.AppendText($"{assembly.FullName}{(!assembly.IsDynamic ? $"@{assembly.Location}" : string.Empty)}\n");
                    }
                }
                catch (Exception ex)
                {
                    TextBox_Debug.AppendText($"Error retrieving advanced log: {ex}\n");
                }
            }
            else TextBox_Debug.Visibility = Visibility.Collapsed;
            UpdateStats();

            TabItem_DebugOptions.Visibility = Vars.CurrentConf.DebugMode ? Visibility.Visible : Visibility.Hidden;
            this.Title = $"{Vars.ManagementWindowDefaultTitle}{(Vars.CurrentConf.DebugMode ? " *用户调试模式*" : string.Empty)}{(Vars.CurrentConf.SuppressLogOutput ? " *日志已被压制*" : string.Empty)}{(Debugger.IsAttached ? " *弱智模式*" : string.Empty)}";
        }

        private async void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            await DarkenAsync();
            await Apply();
            await BrightenAsync();
        }

        private void Button_Donate_Click(object sender, RoutedEventArgs e)
        {
            donateWindow.IsOpen = true;
        }

        private void Button_CheckUpd_Click(object sender, RoutedEventArgs e)
        {
            updateWindow.IsOpen = true;
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
            TextBlock_TTSPitch.Text = Math.Round(Slider_TTSPitch.Value).ToString();
            TextBlock_GiftThrottleDuration.Text = Math.Round(Slider_GiftThrottleDuration.Value).ToString();
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
            windowClosed = false;

            await OnLoad(null, null);
            _updateSliderAllowed = true;
            UpdateSliders(null, null);
            // update information in the thanks field
            // THIS IS NOT TELEMETRY, WE RESPECT OUR USERS' PRIVACY AND WILL NEVER DO SO
            if (Vars.CurrentConf.AllowDownloadMessage)
            {
                WebBrowser_Today.Navigate("https://static-cn.itsmy.app:12306/files/today.html");
            }
            else
            {
                WebBrowser_Today.Visibility = Visibility.Hidden;
            }
        }

        private void Button_DeleteCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (FileInfo fileInfo in (new DirectoryInfo(Vars.CacheDir)).GetFiles())
                {
                    fileInfo.Delete();
                }
                AsyncDialog.Open("OK");
                TextBox_ExecutionResult.Text = "已清理缓存";
            }
            catch (Exception ex)
            {
                AsyncDialog.Open("Error: " + ex.ToString(), "Re: TTSCat", MessageBoxIcon.Error);
                TextBox_ExecutionResult.Text = $"缓存清理失败: {ex.Message}";
            }
        }

        private void Button_ManuallyCrash_Click(object sender, RoutedEventArgs e)
        {
            TextBox_ExecutionResult.Text = "要崩溃啦... ヽ(*。>Д<)o゜";
            throw new NullReferenceException();
        }

        private void Button_StartDebugger_Click(object sender, RoutedEventArgs e)
        {
            TextBox_ExecutionResult.Text = "要崩溃啦... ┑(￣Д ￣)┍";
            Debugger.Launch();
        }

        private void Button_PlayAudio_Click(object sender, RoutedEventArgs e)
        {
            var frame = new DispatcherFrame();
            var thread = new Thread(() => {
                try
                {
                    var waveOut = new WaveOutEvent();
                    var reader = new AudioFileReader(Interaction.InputBox("输入文件名", "Re: TTSCat"));
                    waveOut.Init(reader);
                    waveOut.Play();
                    frame.Continue = false;
                }
                catch (Exception ex)
                {
                    AsyncDialog.Open($"Error: {ex}", "Re: TTSCat", MessageBoxIcon.Error);
                    Dispatcher.Invoke(() => { TextBox_ExecutionResult.Text = $"播放错误: {ex.Message}"; });
                }
            });
            thread.Start();
            TextBox_ExecutionResult.Text = "已启动播放";
            Dispatcher.PushFrame(frame);
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
            CheckBox_AutoUpdate.IsEnabled = false;
            if ((statsUpdater == null) || !statsUpdater.IsAlive)
            {
                statsUpdater = new Thread(() => UpdateStatsThread());
            }
            statsUpdater.Start();
            CheckBox_AutoUpdate.IsEnabled = true;
        }

        private void CheckBox_AutoUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox_AutoUpdate.IsEnabled = false;
            statsUpdater.Abort();
            CheckBox_AutoUpdate.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowClosed = true;
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
            int totalCleared = 0;
            lock (TTSPlayer.fileList)
            {
                totalCleared = TTSPlayer.fileList.Count;
                TTSPlayer.fileList.Clear();
            }
            TextBox_ExecutionResult.Text = $"已取消 {totalCleared} 个待播放语音";
        }

        private void Button_ClearCache_Click(object sender, RoutedEventArgs e)
        {
            if (TTSPlayer.fileList.Count > 0)
            {
                AsyncDialog.Open($"还有 {TTSPlayer.fileList.Count} 个语音在队列中，删除可能导致插件工作异常\n\n请在播放完毕后再试一次", icon: MessageBoxIcon.Warning);
                return;
            }
            try
            {
                long totalSize = 0;
                int count = 0;
                foreach (FileInfo fileInfo in (new DirectoryInfo(Vars.CacheDir)).GetFiles())
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
                AsyncDialog.Open($"出错: {ex}", icon: MessageBoxIcon.Error);
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

        private void ComboBox_PostMethod_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_PostMethod.SelectedIndex == 0)
            {
                TextBlock_PostData.Visibility = Visibility.Hidden;
                TextBox_PostData.Visibility = Visibility.Hidden;
                TextBlock_Headers.Visibility = Visibility.Hidden;
                TextBox_Headers.Visibility = Visibility.Hidden;
            }
            else
            {
                TextBlock_PostData.Visibility = Visibility.Visible;
                TextBox_PostData.Visibility = Visibility.Visible;
                TextBlock_Headers.Visibility = Visibility.Visible;
                TextBox_Headers.Visibility = Visibility.Visible;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Vars.HangWhenCrash = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Vars.HangWhenCrash = false;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.danmuji.org/plugins/Re-TTSCat#%E8%87%AA%E5%AE%9A%E4%B9%89%E5%A4%B4%E8%A1%94");
        }

        private void Button_TriggerGc_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            TextBox_ExecutionResult.Text = "垃圾回收成功";
        }

        private void ComboBox_Engine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Engine.SelectedIndex == 6)
            {
                ComboBox_Person.IsEnabled = true;
                Slider_TTSPitch.IsEnabled = true;
            }
            else
            {
                ComboBox_Person.IsEnabled = false;
                Slider_TTSPitch.IsEnabled = false;
            }
            if (ComboBox_Engine.SelectedIndex == 6 || ComboBox_Engine.SelectedIndex == 1) Slider_TTSSpeed.IsEnabled = true;
            else Slider_TTSSpeed.IsEnabled = false;
        }

        private void Button_DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_VoiceReplyRules.SelectedIndex != -1)
            {
                voiceReplyRulesDataSource.RemoveAt(ListView_VoiceReplyRules.SelectedIndex);
            }
        }

        private void Button_AddRule_Click(object sender, RoutedEventArgs e)
        {
            voiceReplyRulesDataSource.Add(new VoiceReplyRule());
            ListView_VoiceReplyRules.SelectedIndex = voiceReplyRulesDataSource.Count - 1;
        }

        private void TextBox_ReplyContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            var obj = (System.Windows.Controls.TextBox)sender;
            if (obj == null) return;
            if (string.IsNullOrWhiteSpace(obj.Text)) obj.Background = Brushes.LightPink;
            else obj.Background = !((sender as System.Windows.Controls.TextBox).DataContext as VoiceReplyRule).IsVariablesGood()
                ? Brushes.LightGoldenrodYellow
                : Brushes.LightGreen;
        }

        private void TextBlock_TTSInQueue_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                lock (TTSPlayer.fileList)
                {
                    TTSPlayer.fileList.Clear();
                }
                UpdateStats();
            }
        }

        private async void Button_TestVoiceReply_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            var context = (sender as System.Windows.Controls.Button).DataContext as VoiceReplyRule;
            var model = await TestVoiceReplyParamsWindow.GetDanmakuModel(context.ReplyContent);
            Activate();
            if (model == null) return;
            model.MsgType = ((VoiceReplyRule.MatchSource)context.MatchingSource == VoiceReplyRule.MatchSource.GiftName)
                ||
                ((VoiceReplyRule.MatchSource)context.MatchingSource == VoiceReplyRule.MatchSource.GiftName) ? MsgTypeEnum.GiftSend : MsgTypeEnum.Comment;
            await TTSPlayer.PlayVoiceReply(model, context, true, true);
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            TabControl_Main.SelectedIndex = 4;
        }

        private void Hyperlink_Click_2(object sender, RoutedEventArgs e)
        {
            Process.Start("https://ai.baidu.com/tech/speech/tts_online");
        }
    }
}
