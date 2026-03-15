using System.ComponentModel.DataAnnotations;

namespace To_Do_app_server.Models.SoftDelete
{
    public class SoftDeletableBase : ISoftDeletable
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt {  get; set; }

        [Required]
        public bool Deleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public void SetDeleted()
        {
            Deleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
