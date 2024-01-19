using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D.W.C.Lib.D.W.C.Models
{
    public class WorkItemDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rev")]
        public int Rev { get; set; }

        [JsonProperty("fields")]
        public WorkItemFields Fields { get; set; }
    }

    public class WorkItemFields
    {
        [JsonProperty("System.AreaPath")]
        public string AreaPath { get; set; }

        [JsonProperty("System.TeamProject")]
        public string TeamProject { get; set; }

        [JsonProperty("System.IterationPath")]
        public string IterationPath { get; set; }

        [JsonProperty("System.WorkItemType")]
        public string WorkItemType { get; set; }

        [JsonProperty("System.State")]
        public string State { get; set; }

        [JsonProperty("System.AssignedTo")]
        public AssignedTo AssignedTo { get; set; }

        [JsonProperty("System.CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("System.Title")]
        public string Title { get; set; }

        [JsonProperty("System.BoardColumn")]
        public string BoardColumn { get; set; }

        [JsonProperty("Microsoft.VSTS.Common.ActivatedDate")]
        public DateTime ActivatedDate { get; set; }

        [JsonProperty("Microsoft.VSTS.Common.ResolvedDate")]
        public DateTime? ResolvedDate { get; set; }

       
    }

    public class AssignedTo
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        
    }
}
