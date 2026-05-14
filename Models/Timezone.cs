using System.ComponentModel.DataAnnotations;

namespace acepickle_chat_api.Models
{
    public class Timezone
    {
        [Key]
        public int Id { get; set; }
        public string Country { get; set; }
        public string TimeZone { get; set; }
        public string Offset { get; set; }
    }
}
