using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuilderSample
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

    public class Fleet
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }

    public class Taxi
    {
        public virtual int Id { get; set; }

        public virtual string LicensePlate { get; set; }

        [Required]
        public virtual Driver Owner { get; set; }

        [Required]
        public virtual Fleet Fleet { get; set; }

        public virtual IList<Order> Orders { get; set; }
    }

    public enum OrderStatus
    {
        Open,
        Taken,
        Complete
    }

    public class Order
    {
        public virtual int Id { get; set; }

        public virtual OrderStatus Status { get; set; }
        public virtual string Address { get; set; }
        public virtual DateTime RequiredTime { get; set; }
        public virtual Taxi AssignedTaxi { get; set; }
    }
}