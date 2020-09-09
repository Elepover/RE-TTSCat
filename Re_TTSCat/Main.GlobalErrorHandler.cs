using Re_TTSCat.Data;
using System;
using System.Linq;
using System.Management;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Re_TTSCat
{
    public partial class Main
    {
        public void GlobalErrorHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Vars.CurrentConf.CatchGlobalError) return;
            if (Vars.HangWhenCrash)
            {
                MessageBox.Show("已捕获弹幕姬崩溃，尝试阻止，请不要关闭此对话框。");
            }
            var obj = (Exception) e.ExceptionObject;
            var window = new Windows.CriticalErrorWindow();
            SystemSounds.Hand.Play();
            window.TextBlock_Header.Text = $"捕捉到关键错误，弹幕姬即将退出:\n{obj.Message}\n此错误可能并非本插件引起，请在反馈前确定您反馈到了正确的作者处。\n如您确定是 Re: TTSCat 所致，请反馈至 plugin-crash@itsmy.app";
            var sb = new StringBuilder($"**CRITICAL ERROR REPORT**\nTime: {DateTime.Now:o}\n\n");
            try
            {
                var wmi = new ManagementObjectSearcher("select * from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
                sb.Append($"OS name: {((string)wmi["Caption"]).Trim()}\n");
                sb.Append($"OS version: {(string)wmi["Version"]}\n\n");
            }
            catch (Exception ex)
            {
                sb.Append($"(Unable to retrieve OS info: {ex.Message})\n\n");
            }
            sb.Append($"Error details:\n{obj}\n\n");
            try
            {
                sb.Append($"CLR: {Environment.Version}\n");
                var assembliesArray = AppDomain.CurrentDomain.GetAssemblies();
                var assemblies = assembliesArray.ToList();
                assemblies.Sort(new AssemblyComparer());
                sb.Append($"Loaded assemblies ({assemblies.Count}): \n");
                foreach (var assembly in assemblies)
                {
                    sb.Append($"{assembly.FullName}{(!assembly.IsDynamic ? $"@{assembly.Location}" : "")}\n");
                }
            }
            catch (Exception ex)
            {
                sb.Append($"(Unable to retrieve assembly info: {ex.Message})\n");
            }
            window.TextBox_ErrorDetails.Text = sb.ToString();
            window.ShowDialog();
        }
    }
}
