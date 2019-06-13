using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static async Task<Conf> ReadAsync(bool apply = true)
        {
            var reader = new StreamReader(Vars.confFileName, Encoding.UTF8);
            var settingsText = await reader.ReadToEndAsync();
            reader.Close();
            var result = JsonConvert.DeserializeObject<Conf>(settingsText);
            if (apply) { Vars.CurrentConf = result; }
            return result;
        }
    }
}
