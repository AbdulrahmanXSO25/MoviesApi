using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;
using MoviesAPI.DTO;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GettAllAsync()
        {
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();


            return Ok(genres);
        }

        [HttpGet(template: "{id}")]
        public async Task<IActionResult> GettAllAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(genres => genres.Id == id);

            if (genre == null) { return NotFound(value: $"NO GENRE WAS FOUND WITH ID {id}"); };

            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDTO dto)
        {
            var genre = new Genre() { Name = dto.Name};

            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();

            return Ok(genre);
        }


        [HttpPut(template:"{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] GenreDTO dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);

            if (genre == null) { return NotFound(value: $"NO GENRE WAS FOUND WITH ID {id}"); }

            genre.Name = dto.Name;
            _context.SaveChanges();

            return Ok(genre);
        }

        [HttpDelete(template: "{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);

            if (genre == null) { return NotFound(value: $"NO GENRE WAS FOUND WITH ID {id}"); }

            _context.Remove(genre);
            _context.SaveChanges();

            return(Ok(genre));
        }

    }
}
