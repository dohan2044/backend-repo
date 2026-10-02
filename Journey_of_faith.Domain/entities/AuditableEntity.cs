using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities
{
    public abstract class AuditableEntity
    {
        public virtual int Id { get; set; }

        public Guid? CreatorUserId { get; set; } = Guid.Empty;
        public DateTime? CreationTime { get; set; }

        public Guid? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public Guid? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }

        public bool? IsDeleted { get; set; } = false;
    }
}
