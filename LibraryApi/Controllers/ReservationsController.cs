using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryApi.Domain;
using LibraryApi.Filters;
using LibraryApi.Models.Reservations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class ReservationsController : ControllerBase
    {
        private readonly LibraryDataContext _context;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;

        public ReservationsController(LibraryDataContext context, IMapper mapper, MapperConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("reservations")]
        [ValidateModel]
        public async Task<ActionResult> AddReservation([FromBody] PostReservationRequest request)
        {
            var reservation = _mapper.Map<Reservation>(request);
            reservation.Status = ReservationStatus.Pending;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<ReservationDetailResponse>(reservation);
            //await Task.Delay(response.Items.Split(',').Count() * 1000);
            //response.AvailableOn = DateTime.Now.AddDays(1);
            return CreatedAtRoute("reservations#getbyid", new { id = response.Id }, response);
        }

        [HttpGet("reservations/{id}", Name ="reservations#getbyid")]
        public async Task<ActionResult> GetReservationById(int id)
        {
            var reservation = await _context.Reservations
                .ProjectTo<ReservationDetailResponse>(_config)
                .SingleOrDefaultAsync(r => r.Id == id);
            return this.Maybe(reservation);
        }
    }
}
