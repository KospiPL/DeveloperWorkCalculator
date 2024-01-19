using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D.W.C.Lib.D.W.C.Models
{
    public class AzureDevOpsSettings
    {
        public string PersonalAccessToken { get; set; }
        public string Organization { get; set; }
        public string Project { get; set; }
        public string Team { get; set; }
    }

}
