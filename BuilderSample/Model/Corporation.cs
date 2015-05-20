using System.ComponentModel.DataAnnotations;

namespace BuilderSample.Model
{
    public class Corporation
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual string Name { get; set; }
    }
}