using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace softskiller_chat_api.Models
{
    public class User : Created
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Name { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? FirstName { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? LastName { get; set; }
        public int? Platform { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Role { get; set; }
        public string? DeviceId { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Folder { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? VerifiedMobile { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? VerifiedEmail { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? TFA_Mobile { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? TFA_Email { get; set; }
   
        [System.Text.Json.Serialization.JsonIgnore]
        public string? RefreshToken { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? RefreshToken_ExpiryTime { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string? PasswordResetToken { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? PasswordTokenCreated { get; set; }
         
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? Last_Login {get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? ProfileCompleation { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? OnboardingGuidance { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? CompletedSignupWizardCount { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string? IPAddress { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string? GeoLocation { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int? TimeZoneId { get; set; }


    }

    public class UserDetails : Updated
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int UserId { get; set; }
        public string? FatherName { get; set; }
        public DateTime? DOB { get; set; }
        public int? Gender { get; set; }
        public string? Mobile { get; set; }
        public string? MobileAlternate { get; set; }
        public string? EmailAlternate { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? DOJ { get; set; }
        public string? Designation { get; set; }
        public string? PermanentAddress { get; set; }
        public string? PermanentCity { get; set; }
        public string? PermanentState { get; set; }
        public string? PermanentAddressPincode {get; set;}
        public string? PermanentCountry { get; set; }
        public string? CurrentAddress { get; set; }
        public string? CurrentCity { get; set; }
        public string? CurrentState { get; set; }
        public string? CurrentAddressPincode { get; set;}
        public string? CurrentCountry { get; set; }
        public DateTime? DOR { get; set; }
        public string? Remarks { get; set; }
        public sbyte? AccountTypeId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public sbyte? TotalSignupWizardCount { get; set; }

    }

    public class UserDocument : Updated
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [System.Text.Json.Serialization.JsonIgnore]
        public int UserId { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile file { get; set; }
    }

    public class SkillSet : Updated
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public sbyte Status { get; set; }
    }
}