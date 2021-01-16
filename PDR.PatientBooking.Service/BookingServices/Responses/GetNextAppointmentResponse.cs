using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices.Responses
{
    public class GetNextAppointmentResponse
    {
        public List<Booking> Bookings { get; set; }

        public class Booking
        {
            public Guid Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public long PatientId { get; set; }
            public long DoctorId { get; set; }
            public int SurgeryType { get; set; }
        }
    }
}
