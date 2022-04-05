using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.API.Entities;
using Movies.API.Persistence;

namespace Movies.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize("ClientIdPolicy")]
	[Authorize("ScopePolicy")]
	//[Authorize]
	public class MoviesController : ControllerBase
	{
		private readonly MoviesDbContext _context;

		public MoviesController(MoviesDbContext context)
		{
			_context = context;
		}

		// GET: api/Movies
		[HttpGet(Name = nameof(GetMovies))]
		public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
		{
			return await _context.Movies.ToListAsync();
		}

		// GET: api/Movies/5
		[HttpGet("{id}", Name = nameof(GetMovie))]
		public async Task<ActionResult<Movie>> GetMovie(int id)
		{
			var movie = await _context.Movies.FindAsync(id);

			if (movie == null)
			{
				return NotFound();
			}

			return movie;
		}

		// PUT: api/Movies/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for
		// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
		[HttpPut("{id}", Name = nameof(PutMovie))]
		public async Task<IActionResult> PutMovie(int id, Movie movie)
		{
			if (id != movie.Id)
			{
				return BadRequest();
			}

			_context.Entry(movie).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MovieExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Movies
		// To protect from overposting attacks, enable the specific properties you want to bind to, for
		// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
		[HttpPost(Name = nameof(PostMovie))]
		public async Task<ActionResult<Movie>> PostMovie(Movie movie)
		{
			_context.Movies.Add(movie);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
		}

		// DELETE: api/Movies/5
		[HttpDelete("{id}", Name = nameof(DeleteMovie))]
		public async Task<ActionResult<Movie>> DeleteMovie(int id)
		{
			var movie = await _context.Movies.FindAsync(id);
			if (movie == null)
			{
				return NotFound();
			}

			_context.Movies.Remove(movie);
			await _context.SaveChangesAsync();

			return movie;
		}

		private bool MovieExists(int id)
		{
			return _context.Movies.Any(e => e.Id == id);
		}
	}
}