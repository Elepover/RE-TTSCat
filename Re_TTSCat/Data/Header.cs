using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Re_TTSCat.Data
{
    public class Header
    {
        public string Name;
        public string Value;
        public Header() { }
        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
