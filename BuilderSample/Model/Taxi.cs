using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuilderSample.Model
{
    public class Taxi
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual string LicensePlate { get; set; }

        [Required]
        public virtual Driver Owner { get; set; }

        [Required]
        public virtual Corporation Corporation { get; set; }

        public virtual IList<Order> Orders { get; set; }
    }
}