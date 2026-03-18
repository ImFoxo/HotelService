namespace To_Do_app_server.Models.SoftDelete
{
    public interface ISoftDeletable
    {
        bool Deleted {  get; set; }
        DateTime? DeletedAt { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? LastUpdatedAt { get; set; }
    }
}
