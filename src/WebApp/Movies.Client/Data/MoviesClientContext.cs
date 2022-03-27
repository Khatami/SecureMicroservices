using Microsoft.EntityFrameworkCore;
using Movies.Client.Models;

namespace Movies.Client.Data
{
	public class MoviesClientContext : DbContext
	{
		public MoviesClientContext(DbContextOptions<MoviesClientContext> options)
			: base(options)
		{
		}

		public DbSet<Movie> Movie { get; set; }
	}
}
