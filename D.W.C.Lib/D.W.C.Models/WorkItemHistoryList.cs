using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D.W.C.Lib.D.W.C.Models
{
    public class WorkItemHistoryList
    {
        [JsonProperty("count")]
        public int? count {  get; set; }
        [JsonProperty("value")]
        public List<WorkItemHistory>? Value { get; set; }
    }
    public class WorkItemHistory
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("rev")]
        public int Rev { get; set; }
        [JsonProperty("fields")]
        public Fields? Fields { get; set; }
    }

    public class Fields
    {
        [JsonProperty("System.ChangedDate")]
        public ChangeDate? System_ChangedDate { get; set; }
        [JsonProperty("System.BoardColumn")]
        public BoardColumn? System_BoardColumn { get; set; }
    }

    public class ChangeDate
    {
        [JsonProperty("oldValue")]
        public DateTime? OldValue { get; set; }
        [JsonProperty("newValue")]
        public DateTime? NewValue { get; set; }
    }

    public class BoardColumn
    {
        [JsonProperty("oldValue")]
        public string? OldValue { get; set; }
        [JsonProperty("newValue")]
        public string? NewValue { get; set; }
    }

}
