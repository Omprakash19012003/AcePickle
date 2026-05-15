using Microsoft.EntityFrameworkCore;
using acepickle_chat_api.Models;

namespace acepickle_chat_api.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext()
        {
        }
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDetails> UserDetails { get; set; }
        public virtual DbSet<ChatRoom> ChatRooms { get; set; }
        public virtual DbSet<ChatRoomMapping> ChatRoomMappings { get; set; }
        public virtual DbSet<ChatMessages> ChatMessages { get; set; }
        public virtual DbSet<ActiveUser> ActiveUsers { get; set; }
        public virtual DbSet<Timezone> TimeZones { get; set; }
        public virtual DbSet<ActiveDevices> ActiveDevices { get; set; }
        public virtual DbSet<CourseMapping> CourseMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<ActiveUser>(entity =>
               {
                   entity.HasKey(e => e.Id).HasName("PRIMARY");
                   entity.ToTable("ActiveUsers");

                   entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

                   entity.Property(e => e.UserId).HasColumnType("int(11)").HasColumnName("userid");

                   entity.Property(e => e.ConnectionId).HasMaxLength(50).HasColumnName("connection_id");

                   entity.Property(e => e.UserStatus).HasColumnName("user_status");

                   entity.Property(e => e.LastOnline).HasColumnName("last_online");

               });


            modelBuilder.Entity<ChatRoom>(entity =>
               {
                   entity.HasKey(e => e.Id).HasName("PRIMARY");
                   entity.ToTable("ChatRoom");

                   entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

                   entity.Property(e => e.RoomId).HasMaxLength(50).HasColumnName("room_id");

                   entity.Property(e => e.BatchId).HasColumnType("int(11)").HasColumnName("batchId");

                   entity.Property(e => e.RoomName).HasMaxLength(255).HasColumnName("roomName");

                   entity.Property(e => e.CreatedDate).HasColumnName("created_date");

               });

            modelBuilder.Entity<ChatRoomMapping>(entity =>
              {
                  entity.HasKey(e => e.Id).HasName("PRIMARY");
                  entity.ToTable("ChatRoomMapping");

                  entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

                  entity.Property(e => e.RoomId).HasMaxLength(50).HasColumnName("roomId");

                  entity.Property(e => e.UserId).HasColumnType("int(11)").HasColumnName("userid");

                  entity.Property(e => e.RoomType).HasColumnType("int(11)").HasColumnName("room_type");

                  entity.Property(e => e.Status).HasColumnName("status");

                  entity.Property(e => e.CreatedDate).HasColumnName("created_date");

              });

            modelBuilder.Entity<ChatMessages>(entity =>
              {
                  entity.HasKey(e => e.Id).HasName("PRIMARY");
                  entity.ToTable("ChatMessages");

                  entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

                  entity.Property(e => e.RoomId).HasMaxLength(50).HasColumnName("roomId");

                  entity.Property(e => e.UserId).HasColumnType("int(11)").HasColumnName("userId");

                  entity.Property(e => e.Message).HasMaxLength(255).HasColumnName("messages");

                  entity.Property(e => e.Status).HasColumnName("status");

                  entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                  entity.Property(e => e.IsEdited).HasColumnName("isEdited").HasColumnType("tinyint(1)").HasDefaultValue(false);

                  entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");

              });

            modelBuilder.Entity<User>(entity =>
           {
               entity.HasKey(e => e.Id).HasName("PRIMARY");

               entity.ToTable("User");

               entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

               entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email").IsRequired();

               entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("password");

               entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");

               entity.Property(e => e.FirstName).HasMaxLength(255).HasColumnName("firstName");

               entity.Property(e => e.LastName).HasMaxLength(255).HasColumnName("lastName");

               entity.Property(e => e.Platform).HasColumnType("int(11)").HasColumnName("platform");

               entity.Property(e => e.Role).HasMaxLength(255).HasColumnName("roles");

               entity.Property(e => e.DeviceId).HasMaxLength(255).HasColumnName("deviceId");

               entity.Property(e => e.Folder).HasMaxLength(255).HasColumnName("Folder");

               entity.Property(e => e.CreatedOn).HasColumnName("created_date");

               entity.Property(e => e.CreatedBy).HasColumnType("int(11)").HasColumnName("created_by");

               entity.Property(e => e.VerifiedMobile).HasColumnName("verifiedMobile");

               entity.Property(e => e.VerifiedEmail).HasColumnName("verifiedEmail");

               entity.Property(e => e.TFA_Mobile).HasColumnName("tfa_Mobile");

               entity.Property(e => e.TFA_Email).HasColumnName("tfa_Email");

               entity.Property(e => e.RefreshToken).HasMaxLength(255).HasColumnName("refresh_token");

               entity.Property(e => e.RefreshToken_ExpiryTime).HasColumnName("refresh_token_expiryTime");

               entity.Property(e => e.PasswordResetToken).HasMaxLength(300).HasColumnName("PasswordResetToken");

               entity.Property(e => e.PasswordTokenCreated).HasColumnName("PasswordTokenCreated");

               entity.Property(e => e.Last_Login).HasColumnType("datetime").HasColumnName("last_Login");

               entity.Property(e => e.ProfileCompleation).HasColumnName("profile_compleation");

               entity.Property(e => e.OnboardingGuidance).HasColumnName("onboarding_guidance");

               entity.Property(e => e.CompletedSignupWizardCount).HasColumnName("completed_wizard_count");

               entity.Property(e => e.IPAddress).HasMaxLength(255).HasColumnName("ipAddress");

               entity.Property(e => e.GeoLocation).HasMaxLength(255).HasColumnName("geolocation");

               entity.Property(e => e.TimeZoneId).HasColumnName("timeZoneId");

           });

            modelBuilder.Entity<UserDetails>(entity =>
           {
               entity.HasKey(e => e.Id).HasName("PRIMARY");

               entity.ToTable("UserDetails");

               entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

               entity.Property(e => e.UserId).HasColumnType("int(11)").HasColumnName("userid");

               entity.Property(e => e.FatherName).HasMaxLength(255).HasColumnName("fathername");

               entity.Property(e => e.DOB).HasColumnName("dob");

               entity.Property(e => e.Gender).HasColumnType("int(11)").HasColumnName("gender");

               entity.Property(e => e.Mobile).HasMaxLength(255).HasColumnName("mobile");

               entity.Property(e => e.MobileAlternate).HasMaxLength(255).HasColumnName("mobilealternate");

               entity.Property(e => e.EmailAlternate).HasMaxLength(255).HasColumnName("emailalternate");

               entity.Property(e => e.ProfilePicture).HasMaxLength(255).HasColumnName("profilepicture");

               entity.Property(e => e.DOJ).HasColumnName("doj");

               entity.Property(e => e.Designation).HasMaxLength(255).HasColumnName("designation");

               entity.Property(e => e.PermanentAddress).HasMaxLength(255).HasColumnName("permanentaddress");

               entity.Property(e => e.PermanentCity).HasMaxLength(255).HasColumnName("permanentcity");

               entity.Property(e => e.PermanentState).HasMaxLength(255).HasColumnName("permanentstate");

               entity.Property(e => e.PermanentAddressPincode).HasMaxLength(15).HasColumnName("permanentaddresspincode");

               entity.Property(e => e.PermanentCountry).HasMaxLength(255).HasColumnName("permanentcountry");

               entity.Property(e => e.CurrentAddress).HasMaxLength(255).HasColumnName("currentaddress");

               entity.Property(e => e.CurrentCity).HasMaxLength(255).HasColumnName("currentcity");

               entity.Property(e => e.CurrentState).HasMaxLength(255).HasColumnName("currentstate");

               entity.Property(e => e.CurrentAddressPincode).HasMaxLength(15).HasColumnName("currentaddresspincode");

               entity.Property(e => e.CurrentCountry).HasMaxLength(255).HasColumnName("currentcountry");

               entity.Property(e => e.DOR).HasColumnName("dor");

               entity.Property(e => e.Remarks).HasMaxLength(255).HasColumnName("remarks");

               entity.Property(e => e.AccountTypeId).HasColumnName("account_typeid");

               entity.Property(e => e.TotalSignupWizardCount).HasColumnName("total_wizard_count");

           });

            modelBuilder.Entity<Timezone>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("Timezone");
                entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");
                entity.Property(e => e.Country).HasColumnType("varchar(100)").HasColumnName("Country");
                entity.Property(e => e.TimeZone).HasColumnType("varchar(100)").HasColumnName("Timezone");
                entity.Property(e => e.Offset).HasColumnType("varchar(100)").HasColumnName("offset");
            });

            modelBuilder.Entity<ActiveDevices>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("activedevices");

                entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnType("int(11)").HasColumnName("userid");
                entity.Property(e => e.PlatformId).HasColumnType("int(11)").HasColumnName("platformid");
                entity.Property(e => e.DeviceToken).HasMaxLength(500).HasColumnName("devicetoken");
                entity.Property(e => e.RefreshToken).HasMaxLength(500).HasColumnName("refreshtoken");
                entity.Property(e => e.Expirytime).HasColumnType("datetime").HasColumnName("expirytime");
                entity.Property(e => e.IpAddress).HasMaxLength(500).HasColumnName("ipaddress");
                entity.Property(e => e.Location).HasMaxLength(500).HasColumnName("location");
                entity.Property(e => e.ActiveStatus).HasColumnName("activeStatus");
                entity.Property(e => e.CreatedTime).HasColumnType("datetime").HasColumnName("created_datetime");
                entity.Property(e => e.UpdatedTime).HasColumnType("datetime").HasColumnName("updated_datetime");
            });

            modelBuilder.Entity<CourseMapping>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("CourseMappings");

                entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("id");

                entity.Property(e => e.Userid).HasColumnType("int(11)").HasColumnName("userid");

                entity.Property(e => e.Courseid).HasColumnType("int(11)").HasColumnName("courseid");

                entity.Property(e => e.Orderid).HasColumnType("int(11)").HasColumnName("orderid");

                entity.Property(e => e.Batchid).HasColumnType("int(11)").HasColumnName("batchid");

                entity.Property(e => e.View_Recordings).HasColumnName("view_recordings");

                entity.Property(e => e.Lock_Tests).HasColumnName("lock_test");

                entity.Property(e => e.PaymentType).HasColumnName("payment_type");

                entity.Property(e => e.PurchasedOn).HasColumnName("purchasedOn");

                entity.Property(e => e.Timespent).HasColumnType("decimal(10,2)").HasColumnName("timespent");

                entity.Property(e => e.Validity).HasColumnName("validity");

                entity.Property(e => e.Price).HasColumnType("int(11)").HasColumnName("price");

                entity.Property(e => e.Status).HasColumnName("status");

            });
        }
    }
}