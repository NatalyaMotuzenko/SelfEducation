using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EvergreenLibrary.Validation
{
    public static class CustomValidation
    {
        public sealed class CheckYear : ValidationAttribute
        {
            protected override ValidationResult IsValid(object year, ValidationContext validationContext)
            {
                var currentYear = Convert.ToInt32(DateTime.Now.Year);
                if (Convert.ToInt32(year) >= 0 && Convert.ToInt32(year) <= currentYear)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Year does not exist.");
                }
            }
        }
    }
}