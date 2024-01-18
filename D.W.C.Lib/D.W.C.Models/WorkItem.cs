using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWorkCalc.D.W.C.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }

}
