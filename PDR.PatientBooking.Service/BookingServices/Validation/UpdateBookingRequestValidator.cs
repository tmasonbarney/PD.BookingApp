using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class UpdateBookingRequestValidator : IUpdateBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public UpdateBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(UpdateBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (OrderDoesNotExist(request, ref result))
                return result;

            return result;
        }

        private bool MissingRequiredFields(UpdateBookingRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (request.Id == Guid.Empty)
                errors.Add("Must be a valid Id");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool OrderDoesNotExist(UpdateBookingRequest request, ref PdrValidationResult result)
        {

            if (_context.Order.Any(x => x.Id.Equals(request.Id)))
            {
                return false;

            }


            result.PassedValidation = false;
            result.Errors.Add("Order does not exist");
            return true;
        }
    }


}

