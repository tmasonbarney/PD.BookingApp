using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using static PDR.PatientBooking.Service.BookingServices.Responses.GetNextAppointmentResponse;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _addBookingValidator;
        private readonly IUpdateBookingRequestValidator _updateBookingRequestValidator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator addBookingValidator, IUpdateBookingRequestValidator updateBookingRequestValidator)
        {
            _context = context;
            _addBookingValidator = addBookingValidator;
            _updateBookingRequestValidator = updateBookingRequestValidator;
        }

        public void AddBooking(AddBookingRequest request)
        {
            var validationResult = _addBookingValidator.ValidateRequest(request);

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
                SurgeryType = (int)_context.Patient.FirstOrDefault(x => x.Id == request.PatientId).Clinic.SurgeryType,
                Status = (int)BookingStatus.Open
            });

            _context.SaveChanges();
        }

        public UpdateBookingResponse UpdateBooking(UpdateBookingRequest request)
        {
            //Get order
            var validationResult = _updateBookingRequestValidator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var order = _context.Order.First(x => x.Id.Equals(request.Id));

            //Update
            order.StartTime = request.StartTime;
            order.EndTime = request.EndTime;
            order.PatientId = request.PatientId;
            order.Patient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId);
            order.DoctorId = request.DoctorId;
            order.Doctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId);
            order.SurgeryType = (int)_context.Patient.FirstOrDefault(x => x.Id == request.PatientId).Clinic.SurgeryType;
            order.Status = request.Status;

            //Save
            _context.SaveChanges();

            return new UpdateBookingResponse
            {
                Booking = new Booking
                {
                    StartTime = order.StartTime,
                    EndTime = order.EndTime,
                    PatientId = order.PatientId,
                    DoctorId = order.DoctorId,
                    SurgeryType = order.SurgeryType,
                    Status = (BookingStatus)order.Status
                }
            };
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
                    var bookings3 = bookings2.Where(x => x.StartTime > DateTime.Now && x.Status != (int)BookingStatus.Cancelled);


                   
                    var nextBooking = new Booking
                    {
                        Id = bookings3.First().Id,
                        PatientId = bookings3.First().PatientId,
                        SurgeryType = bookings3.First().SurgeryType,
                        DoctorId = bookings3.First().DoctorId,
                        StartTime = bookings3.First().StartTime,
                        EndTime = bookings3.First().EndTime,
                        Status = (BookingStatus)bookings3.First().Status
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