using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.BookingServices;

using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using System.Linq;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{
    [TestFixture]
    public class BookingServiceTests
    {
        private MockRepository _mockRepository;
        private IFixture _fixture;

        private PatientBookingContext _context;
        private Mock<IAddBookingRequestValidator> _validator;

        private BookingService _bookingService;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fixture = new Fixture();

            //Prevent fixture from generating circular references
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _validator = _mockRepository.Create<IAddBookingRequestValidator>();

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _bookingService = new BookingService(
                _context,
                _validator.Object
            );
        }

        private void SetupMockDefaults()
        {
            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddBookingRequest>()))
                .Returns(new PdrValidationResult(true));
        }

        [Test]
        public void AddBooking_ValidatesRequest()
        {
            //arrange
            var request = _fixture.Create<AddBookingRequest>();

            //act
            _bookingService.AddBooking(request);

            //assert
            _validator.Verify(x => x.ValidateRequest(request), Times.Once);
        }

        [Test]
        public void AddClinic_ValidatorFails_ThrowsArgumentException()
        {
            //arrange
            var failedValidationResult = new PdrValidationResult(false, _fixture.Create<string>());

            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddBookingRequest>())).Returns(failedValidationResult);

            //act
            var exception = Assert.Throws<ArgumentException>(() => _bookingService.AddBooking(_fixture.Create<AddBookingRequest>()));

            //assert
            exception.Message.Should().Be(failedValidationResult.Errors.First());
        }

        [Test]
        public void AddClinic_AddsClinicToContextWithGeneratedId()
        {
            //arrange
            var request = _fixture.Create<AddBookingRequest>();

            var expected = new Booking
            {
                Name = request.Name,
                SurgeryType = request.SurgeryType,
                Patients = new List<Patient>()
            };

            //act
            _bookingService.AddBooking(request);

            //assert
            _context.Clinic.Should().ContainEquivalentOf(expected, options => options.Excluding(clinic => clinic.Id));
        }

        [Test]
        public void GetNextPatientAppointment_NoAppointments_ReturnsNull()
        {
            //arrange
            long id = 1;

            //act
            var res = _bookingService.GetPatientsNextAppointment(id);

            //assert
            res.Should().BeNull();
        }

        [Test]
        public void GetNextPatientAppointment_ReturnsAppointment()
        {
            //arrange
            var clinic = _fixture.Create<Booking>();
            _context.Clinic.Add(clinic);
            _context.SaveChanges();

            var expected = new Appointment
            {
                
            };

            //act
            var res = _clinicService.GetAllClinics();

            //assert
            res.Should().BeEquivalentTo(expected);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
