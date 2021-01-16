using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using static PDR.PatientBooking.Service.BookingServices.Responses.GetNextAppointmentResponse;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _validator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public void AddBooking(AddBookingRequest request)
        {
            var validationResult = _validator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            _context.Order.Add(new Order
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientId = request.PatientId,
                Patient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId),
                DoctorId = request.DoctorId,
                Doctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId),
                SurgeryType = (int)_context.Patient.FirstOrDefault(x => x.Id == request.PatientId).Clinic.SurgeryType
            });

            _context.SaveChanges();
        }

    

        public GetNextAppointmentResponse GetPatientsNextAppointment(long id)
        {
            var bockings = _context.Order.OrderBy(x => x.StartTime).ToList();

            if (bockings.Where(x => x.Patient.Id == id).Count() == 0)
            {
                return null;
            }
            else
            {
                var bookings2 = bockings.Where(x => x.PatientId == id);
                if (bookings2.Where(x => x.StartTime > DateTime.Now).Count() == 0)
                {
                    return null;
                }
                else
                {
                    var bookings3 = bookings2.Where(x => x.StartTime > DateTime.Now);

                    var nextBooking = new Booking
                    {
                        Id = bookings3.First().Id,
                        PatientId = bookings3.First().PatientId,
                        SurgeryType = bookings3.First().SurgeryType,
                        DoctorId = bookings3.First().DoctorId,
                        StartTime = bookings3.First().StartTime,
                        EndTime = bookings3.First().EndTime,
                    };

                    return new GetNextAppointmentResponse
                    {
                       Bookings = new List<Booking>() { nextBooking }
                        
                    };
                }
            }
        }
    }
}