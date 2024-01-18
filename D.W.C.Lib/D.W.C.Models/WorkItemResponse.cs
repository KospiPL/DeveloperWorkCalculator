using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWorkCalc.D.W.C.Models
{
    public class WorkItemResponse
    {
        public int Count { get; set; }
        public List<WorkItem> Value { get; set; }
    }

}
