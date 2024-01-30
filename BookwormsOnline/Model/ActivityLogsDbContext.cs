using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
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

	public class ActivityLogsDbContext : DbContext
	{
		private readonly IConfiguration _configuration;

		public ActivityLogsDbContext(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string connectionString = _configuration.GetConnectionString("AuthConnectionString");
			optionsBuilder.UseSqlServer(connectionString);
		}

		public DbSet<LogEntry> LogEntries { get; set; }
	}
}