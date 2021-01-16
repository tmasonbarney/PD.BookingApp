using System;
using System.Collections.Generic;
using System.Text;
using static PDR.PatientBooking.Service.BookingServices.Responses.GetNextAppointmentResponse;

namespace PDR.PatientBooking.Service.BookingServices.Responses
{
    public class UpdateBookingResponse
    {
        public Booking Booking { get; set; }
    }
}
