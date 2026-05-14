using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace softskiller_chat_api.Models
{
    public class CourseMapping : Created
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Required]
        public int Userid { get; set; }
        [Required]
        public int Courseid { get; set; }
        [Required]
        public int? Orderid { get; set; }
        public int? Batchid { get; set; }
        public int? View_Recordings { get; set; }
        public int? Lock_Tests { get; set; }
        public byte? PaymentType { get; set; }
        public DateTime PurchasedOn { get; set; } = DateTime.Now;
        public decimal? Timespent { get; set; } = 0.00m;
        [Required]
        public DateTime Validity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public sbyte Status { get; set; }
    }
}