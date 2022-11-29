using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;
using MoviesAPI.DTO;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        private new List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };

        private long _maxAllowedPosterSize = 8388608/8;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies.Include(g => g.Genre).ToListAsync();


            return Ok(movies);
        }

        [HttpGet(template: "{id}")]
        public async Task<IActionResult> GettAllAsync(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(movies => movies.Id == id);

            if (movie == null) { return NotFound(value: $"NO MOVIE WAS FOUND WITH ID {id}"); };

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDTO dto)
        {
            if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower() ) )
            {
                return BadRequest("Only .jpg and .png images are allowed");
            }
            if (dto.Poster.Length > _maxAllowedPosterSize)
            { 
                return BadRequest("Max Allowed Size For Posters is 8Mb");
            }

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = new Movie()
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Year = dto.Year
            };

            await _context.AddAsync(movie);
            _context.SaveChanges();

            return Ok(movie.Title);
        }

        [HttpPut(template: "{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDTO dto)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null) { return NotFound(value: $"NO MOVIE WAS FOUND WITH ID {id}"); }

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            movie.GenreId = dto.GenreId;
            movie.Title = dto.Title;
            movie.Poster = dataStream.ToArray();
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            movie.Year = dto.Year;

            _context.SaveChanges();

            return Ok(movie.Title);
        }

        [HttpDelete(template: "{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null) { return NotFound(value: $"NO MOVIE WAS FOUND WITH ID {id}"); }

            _context.Remove(movie);
            _context.SaveChanges();

            return (Ok(movie.Title));
        }
    }
}