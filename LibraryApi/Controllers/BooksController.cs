
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryApi.Domain;
using LibraryApi.Filters;
using LibraryApi.Models.Books;
using LibraryApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class BooksController : ControllerBase
    {
        private LibraryDataContext _context;
        private IMapper _mapper;
        private MapperConfiguration _mapperConfig;
        private IQueryForBooks _booksQuery;
        private IDoBookCommands _bookCommands;

        public BooksController(LibraryDataContext context, IMapper mapper, MapperConfiguration mapperConfig, IQueryForBooks booksQuery, IDoBookCommands bookCommands)
        {
            _context = context;
            _mapper = mapper;
            _mapperConfig = mapperConfig;
            _booksQuery = booksQuery;
            _bookCommands = bookCommands;
        }

        [HttpPut("books/{bookId:int}/title")]
        public async Task<ActionResult> UpdateTitle([FromRoute] int bookId, [FromBody] string title)
        {
            bool didUpdate = await _bookCommands.UpdateTitle(bookId, title);
           

            if(didUpdate)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
            
        }


        [HttpDelete("books/{bookId:int}")]
        public async Task<ActionResult> RemoveBookFromInventory(int bookId)
        {
            await _bookCommands.RemoveBook(bookId);

            return NoContent(); // "Idempotent"
        }


        [HttpPost("books")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ValidateModel]
        public async Task<ActionResult<GetBookDetailsResponse>> AddABook([FromBody] PostBookCreate bookToAdd)
        {

                GetBookDetailsResponse response = await _bookCommands.AddBook(bookToAdd);
                // return a 201, with location header, with a copy of what they'd get from that location

                return CreatedAtRoute("books#getbyid", new { bookId = response.Id }, response);

        }



        /// <summary>
        /// Allows you to get a list of our vast inventory of fine books    
        /// </summary>
        /// <returns>A list of books for you to peruse.</returns>
        [HttpGet("/books")]
        [Produces("application/json")]
        public async Task<ActionResult<GetBooksResponse>> GetAllBooks()
        {
            GetBooksResponse response = await _booksQuery.GetAllBooks();

            return Ok(response);

        }

        /// <summary>
        /// Gives you a book for a specific id.
        /// </summary>
        /// <param name="bookId">The id of the book</param>
        /// <returns>Either details about the book or a 404</returns>
        [HttpGet("/books/{bookId:int}", Name = "books#getbyid")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetBookDetailsResponse>> GetBookById([FromRoute] int bookId)
        {
            GetBookDetailsResponse book = await _booksQuery.GetBookById(bookId);

            if (book == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(book);
            }
        }


    }
}
