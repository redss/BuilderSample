using System;
using System.ComponentModel.DataAnnotations;

namespace BuilderSample.Model
{
    public enum OrderStatus
    {
        New,
        Ongoing,
        Completed
    }

    public class Order
    {
        public virtual int Id { get; set; }

        public virtual OrderStatus Status { get; set; }

        [Required]
        public virtual string Address { get; set; }

        public virtual DateTime RequiredTime { get; set; }

        public virtual Taxi AssignedTaxi { get; set; }
    }
}