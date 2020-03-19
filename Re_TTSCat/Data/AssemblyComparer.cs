using System.Collections.Generic;
using System.Reflection;

namespace Re_TTSCat.Data
{
    public class AssemblyComparer : IComparer<Assembly>
    {
        public int Compare(Assembly a, Assembly b)
        {
            return string.Compare(a.FullName, b.FullName);
        }
    }
}
