using System;

namespace BuilderSample.Model
{
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