using BookwormsOnline.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookwormsOnline.Model
{
	public class LogEntry
	{
		public int Id { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Activity { get; set; }

		[Required]
		public DateTime Time { get; set; }
	}

	public class PasswordHistory
	{
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string LoggedPasswordHash { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

    }

	public class AuthDbContext : IdentityDbContext<BookwormsUser>
	{
        private readonly IConfiguration _configuration;
        //public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options){ }
        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); 
			optionsBuilder.UseSqlServer(connectionString);
        }

		public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<PasswordHistory> PasswordHistory { get; set; }
    }

}
