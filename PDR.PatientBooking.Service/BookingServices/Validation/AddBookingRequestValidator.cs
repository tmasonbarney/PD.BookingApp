using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (BookingInPast(request, ref result))
                return result;

            if (DoctorIsAlreadyBooked(request, ref result))
                return result;

            return result;
        }

        private bool BookingInPast(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (request.StartTime < DateTime.Now)
            {
                result.PassedValidation = false;
                result.Errors.Add("Booking cannot be made in the past");
                return true;
            }

            return false;
        }

        private bool DoctorIsAlreadyBooked(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (_context.Doctor.First(x => x.Id == request.DoctorId)
                .Orders.Any(x => x.StartTime >= request.StartTime && x.EndTime <= request.EndTime))
            {

                result.PassedValidation = false;
                result.Errors.Add("Requested Doctor is already booked");
                return true;
            }

            return false;
        }

       
    }
}
