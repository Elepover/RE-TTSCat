using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static async Task InitiateAsync()
        {
            Bridge.ALog("正在载入配置");
            // detect if file exists
            if (!Directory.Exists(Vars.confDir))
            {
                Bridge.Log("未发现配置文件夹，尝试创建中");
                Directory.CreateDirectory(Vars.confDir);
            }
            if (!Directory.Exists(Vars.cacheDir))
            {
                Bridge.Log("未发现缓存文件夹，尝试创建中");
                Directory.CreateDirectory(Vars.cacheDir);
            }
            if (!File.Exists(Vars.confFileName))
            {
                // file does not exist, creating
                Bridge.Log("未发现配置文件，尝试创建中");
                await SaveAsync();
                Bridge.Log("创建成功");
            }
            await ReadAsync();
            Bridge.ALog("正在检查文件");
            try
            {
                if (!File.Exists(Vars.audioLibFileName) || !VerifyLibraryIntegrity())
                {
                    Bridge.ALog("尝试释放音频支持库");
                    var writer = new FileStream(Vars.audioLibFileName, FileMode.OpenOrCreate);
                    await writer.WriteAsync(Properties.Resources.NAudio, 0, Properties.Resources.NAudio.Length);
                    writer.Close();
                }
                if (!Main.IsNAudioReady)
                {
                    Bridge.ALog("正在载入支持库");
                    Assembly.LoadFrom(Vars.audioLibFileName);
                    Main.IsNAudioReady = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"关键错误！\n\n无法正常处理支持库: {ex.Message}\n\n请尝试重新启动弹幕姬，重置配置文件或检查文件读写权限", "关键错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
            Bridge.ALog("载入完毕");
        }
    }
}
