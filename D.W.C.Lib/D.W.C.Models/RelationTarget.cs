using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWorkCalc.D.W.C.Models
{
    public class WorkItemsList
    {
        public List<WorkItemRelation>? WorkItemRelations { get; set; }
    }
    public class WorkItemRelation
    {
        public RelationTarget Target { get; set; }
    }
    public class RelationTarget
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

}
