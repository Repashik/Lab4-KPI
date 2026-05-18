namespace TimeAccountingSystem;

public class Task: WorkBase
{
    // Список етапів виконання завдання
    public List<Stage> Stages = new();
    // Дедлайн та дата створення завдання
    public DateTime? deadLine;
    public DateTime? created;
    // Список коментарів до завдання
    public List<Comment> Comments = new();

    // Структура коментаря
    public struct Comment
    {
        public string Message;
        public User Author;
    }

    public Task(string name, string? description, DateTime? startTime, DateTime? endTime, DateTime? deadLine)
    {
        this.Name = name;
        this.Description = description;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.deadLine = deadLine;
        this.created = DateTime.Now;
    }

    public decimal CalulateProgress()
    {
        if (Stages.Count == 0)
            return 0;
        int totalWeight = Stages.Sum(s => s.Weight);
        int completedWeight = Stages.Where(s => s.EndTime.HasValue).Sum(s => s.Weight);
        return (decimal)completedWeight / totalWeight * 100;
    }

    public TimeSpan CalculateTotalTime() // Змінено тип повернення
    {
        TimeSpan sum = TimeSpan.Zero; // Змінено тип
        foreach (Stage stage in this.Stages)
        {
            if (stage.StartTime != null && stage.EndTime != null)
            {
                sum += stage.EndTime.Value - stage.StartTime.Value;
            }
        }
        return sum;
    }


}
