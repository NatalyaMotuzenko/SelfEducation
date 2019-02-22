using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static EvergreenLibrary.Validation.CustomValidation;

namespace EvergreenLibrary.Infrastructure
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }

        [checkYear]
        public int Year { get; set; } = Convert.ToInt32(DateTime.Now.Year);
        public bool NeedToDelete { get; set; } = false;
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}