using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace softskiller_chat_api.Models
{
    public class Created
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
       
        [System.Text.Json.Serialization.JsonIgnore]
        public int? CreatedBy { get; set; }

    }

    public class Updated : Created
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? LastUpdatedOn { get; set; } = DateTime.Now;
        
        [System.Text.Json.Serialization.JsonIgnore]
        public int? LastUpdatedBy { get; set; }
    }
}