using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static async Task InitiateAsync()
        {
            Bridge.Log("正在载入配置");
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
            Bridge.Log("正在检查文件");
            if (!File.Exists(Vars.audioLibFileName))
            {
                Bridge.Log("尝试释放 NAudio.dll 支持库");
                var writer = new FileStream(Vars.audioLibFileName, FileMode.OpenOrCreate);
                await writer.WriteAsync(Properties.Resources.NAudio, 0, Properties.Resources.NAudio.Length);
                writer.Close();
            }
            Bridge.Log("正在载入支持库");
            Assembly.LoadFrom(Vars.audioLibFileName);
            Main.IsNAudioReady = true;
            Bridge.Log("载入完毕");
        }
    }
}
