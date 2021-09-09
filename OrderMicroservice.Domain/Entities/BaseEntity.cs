using System;

namespace OrderMicroservice.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}