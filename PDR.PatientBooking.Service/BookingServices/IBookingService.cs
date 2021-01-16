using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        void AddBooking(AddBookingRequest request);
        GetNextAppointmentResponse GetPatientsNextAppointment(long id);
        UpdateBookingResponse UpdateBooking(UpdateBookingRequest request);
    }
}
