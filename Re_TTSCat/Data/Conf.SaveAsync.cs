using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static async Task SaveAsync()
        {
            try
            {
                var text = JsonConvert.SerializeObject(Vars.CurrentConf, Vars.CurrentConf.MinifyJson ? Formatting.None : Formatting.Indented);
                var writer = new StreamWriter(Vars.ConfFileName, false, Encoding.UTF8)
                {
                    AutoFlush = true,
                    NewLine = Environment.NewLine
                };
                await writer.WriteAsync(text);
                writer.Close();
            }
            catch (Exception ex)
            {
                Bridge.ALog($"无法保存设置: {ex.Message}");
            }
        }
    }
}
