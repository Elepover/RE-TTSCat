using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Re_TTSCat.Data
{
    public enum RequestType
    {
        JustGet = 0,
        ApplicationXWwwFormUrlencoded = 1,
        MultipartFormData = 2,
        TextPlain = 3
    }
}
