using System;
using System.Threading.Tasks;
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
using System.Runtime.Versioning;

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

        private void Button_CheckConnectivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var frame = new DispatcherFrame();
                var thread = new Thread(() =>
                {
                    var result = "到 Google CN: ";
                    var sw = new Stopwatch();
                    var req = WebRequest.CreateHttp("https://translate.google.cn/");
                    req.Timeout = 5000;
                    sw.Start();
                    req.GetResponse();
                    sw.Stop();
                    result += sw.ElapsedMilliseconds + "ms\n到百度: ";
                    sw.Reset();
                    req = WebRequest.CreateHttp("https://fanyi.baidu.com/");
                    req.Timeout = 5000;
                    sw.Start();
                    req.GetResponse();
                    sw.Stop();
                    result += sw.ElapsedMilliseconds + "ms";
                    frame.Continue = false;
                    AsyncDialog.Open(result, "Re: TTSCat");
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                Dispatcher.PushFrame(frame);
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

        private async Task Apply()
        {
            Vars.CurrentConf.AutoUpdate = CheckBox_AutoUpdates.IsChecked ?? false;
            Vars.CurrentConf.DebugMode = CheckBox_DebugMode.IsChecked ?? false;
            Vars.CurrentConf.DoNotKeepCache = CheckBox_DoNotKeepCache.IsChecked ?? false;
            Vars.CurrentConf.ReadInQueue = CheckBox_ReadInQueue.IsChecked ?? false;
            Vars.CurrentConf.MinimumDanmakuLength = (int)Math.Round(Slider_DMLengthLimit.Value);
            Vars.CurrentConf.MaximumDanmakuLength = (int)Math.Round(Slider_DMLengthLimitMax.Value);
            Vars.CurrentConf.ReadPossibility = (int)Math.Round(Slider_ReadPossibility.Value);
            Vars.CurrentConf.DownloadFailRetryCount = (byte)Math.Round(Slider_RetryCount.Value);
            Vars.CurrentConf.TTSVolume = (int)Math.Round(Slider_TTSVolume.Value);
            Vars.CurrentConf.ReadSpeed = (int)Math.Round(Slider_TTSSpeed.Value);
            Vars.CurrentConf.CustomEngineURL = TextBox_CustomEngineURL.Text;
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
            Slider_DMLengthLimit.Value = Vars.CurrentConf.MinimumDanmakuLength;
            Slider_DMLengthLimitMax.Value = Vars.CurrentConf.MaximumDanmakuLength;
            Slider_ReadPossibility.Value = Vars.CurrentConf.ReadPossibility;
            Slider_RetryCount.Value = Vars.CurrentConf.DownloadFailRetryCount;
            Slider_TTSVolume.Value = Vars.CurrentConf.TTSVolume;
            Slider_TTSSpeed.Value = Vars.CurrentConf.ReadSpeed;
            TextBox_CustomEngineURL.Text = Vars.CurrentConf.CustomEngineURL;
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
            TextBox_Debug.AppendText("Operating system: " + Environment.OSVersion.ToString() + "\n");
            TextBox_Debug.AppendText("CLR: " + Environment.Version.ToString() + "\n");
            TextBox_Debug.AppendText("---------- Plugin Environment ----------\n");
            TextBox_Debug.AppendText("Plugin version: " + Vars.currentVersion.ToString() + "\n");
            TextBox_Debug.AppendText("Plugin executable: " + Vars.dllFileName + "\n");
            TextBox_Debug.AppendText("Plugin configuration directory: " + Vars.confDir + "\n");
            TextBox_Debug.AppendText("Audio library file: " + Vars.audioLibFileName + "\n");
            TextBox_Debug.AppendText("Plugins directory: " + Vars.dllPath + "\n");

            if (Vars.CurrentConf.DebugMode) { TabItem_DebugOptions.Visibility = Visibility.Visible; } else { TabItem_DebugOptions.Visibility = Visibility.Hidden; }
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
            var updateWindow = new UpdateWindow();
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
            Process.Start("https://www.danmuji.org/plugins/Re_TTSCat");
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

            // Your code here.
            await OnLoad(null, null);
            _updateSliderAllowed = true;
            UpdateSliders(null, null);
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
                if (System.Windows.MessageBox.Show("您确定要重置所有设置到默认值吗？", "Re: TSCat", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Vars.CurrentConf = new Conf();
                    Load();
                    System.Windows.MessageBox.Show("已恢复所有设置至默认值，点击保存或应用以保存到配置文件中。", "Re: TSCat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("配置重置失败: " + ex.Message, "Re: TSCat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_PermissionInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("系统权限信息\n\nRe: TTSCat 使用了以下系统权限:\n\n• 读取/写入您的文件系统\n插件需要该权限以进行配置读取和保存及 TTS 文件生成与读取\n\n• 访问互联网\n用于下载 TTS 文件及检查更新", "Re: TTSCat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
