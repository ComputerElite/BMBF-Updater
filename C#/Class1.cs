using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BMBF_Updater
{
    internal class TimeoutWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            // 20 minutes, could probably make it 10
            w.Timeout = 20 * 60 * 1000;
            return w;
        }
    }
}
