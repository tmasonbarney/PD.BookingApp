using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    public class AddBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddBookingRequestValidator _addBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _addBookingRequestValidator = new AddBookingRequestValidator(
                _context
            );
        }

        private void SetupMockDefaults()
        {

        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.Now.AddDays(1);
            request.EndTime = request.StartTime.AddHours(1);

            var order = _fixture.Build<Order>()
                .With(x => x.StartTime, DateTime.Now.AddYears(-1))
                .With(x => x.EndTime, DateTime.Now.AddYears(-1)).Create();

            var doctor = _fixture
                .Build<Doctor>()
                .With(x => x.Id, request.DoctorId)
                .With(x => x.Orders, new List<Order>() { order })
                .Create();

            _context.Doctor.Add(doctor);
            _context.SaveChanges();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        [Test]
        public void ValidateResult_BookingInPast_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.Now.AddDays(-1);
            request.EndTime = request.StartTime.AddHours(1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Booking cannot be made in the past");

        }

        [Test]
        public void ValidateResult_DoctorAlreadyBooked_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.Now.AddDays(1);
            request.EndTime = request.StartTime.AddDays(1).AddHours(1);

            var order = _fixture.Build<Order>()
                .With(x => x.StartTime, request.StartTime)
                .With(x => x.EndTime, request.EndTime)
                .Create();

            var doctor = _fixture
                .Build<Doctor>()
                .With(x => x.Id, request.DoctorId)
                .With(x => x.Orders, new List<Order>() { order })
                .Create();

            _context.Doctor.Add(doctor);
            _context.SaveChanges();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Requested Doctor is already booked");

        }

        private AddBookingRequest GetValidRequest()
        {
            var request = _fixture.Create<AddBookingRequest>();
            return request;
        }
    }
}
