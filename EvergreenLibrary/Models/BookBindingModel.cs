using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EvergreenLibrary.Models
{
    public class PutBookBindingModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
    }

    public class CreateBookBindingModel
    {
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
    }
}