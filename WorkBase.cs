namespace TimeAccountingSystem;

public abstract class WorkBase
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
