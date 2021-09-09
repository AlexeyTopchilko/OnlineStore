using System;

namespace AuthenticationMicroservice.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}