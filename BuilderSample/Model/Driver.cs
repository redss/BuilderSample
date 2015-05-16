using System.ComponentModel.DataAnnotations;

namespace BuilderSample.Model
{
    public class Driver
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual string FirstName { get; set; }

        [Required]
        public virtual string Surname { get; set; }

        //[Required]
        //public string Email { get; set; }
    }
}