using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class BookingModel
    {
        [Key]
        public int BookingId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Persons { get; set; }
        public DateTime Date { get; set; }
    }
}
