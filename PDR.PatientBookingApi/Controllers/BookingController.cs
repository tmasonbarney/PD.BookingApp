using Microsoft.AspNetCore.Mvc;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            
            _bookingService = bookingService;
        }

        [HttpGet("patient/{identificationNumber}/next")]
        public IActionResult GetPatientNextAppointment(long identificationNumber)
        {

            try
            {
                var result = _bookingService.GetPatientsNextAppointment(identificationNumber);

                return result is null ? StatusCode(502) : (IActionResult)Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            
        }

        [HttpPost()]
        public IActionResult AddBooking(AddBookingRequest request)
        {

            try
            {
                _bookingService.AddBooking(request);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        public IActionResult UpdateBooking(UpdateBookingRequest request)
        {
            try
            {
                var result = _bookingService.UpdateBooking(request);
                return Ok(result.Booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

       

    }
}