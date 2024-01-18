using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D.W.C.Lib.D.W.C.Models
{
    public class WorkItemDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public string AreaPath { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? LastChangedDate { get; set; }
    }
}
