namespace TimeAccountingSystem;

public class Manager: Staff
{
    // Конструктор (зсилається на батьківські класи)
    public Manager(string name, string id, decimal salary, decimal bonus)
    : base(name, id, salary, bonus)
    {
    }

    public void CreateTask(string name, string? description, DateTime? startTime, DateTime? endTime, DateTime? deadLine, List<Stage> stages, List<Task> tasks)
    {
        Task newTask = new Task(name, description, startTime, endTime, deadLine);
        newTask.Stages = stages;
        tasks.Add(newTask);
    }

    public void EditTask(Task task, string? name = null, string? description = null, DateTime? startTime = null, DateTime? endTime = null, DateTime? deadLine = null)
    {
        if (name != null)
            task.Name = name;
        if (description != null)
            task.Description = description;
        if (startTime != null)
            task.StartTime = startTime;
        if (endTime != null)
            task.EndTime = endTime;
        if (deadLine != null)
            task.deadLine = deadLine;
    }

    public void DeleteTask(Task task, List<Task> tasks)
    {
        tasks.Remove(task);
    }

    public void AddStage(Task task, string name, int weight)
    {
        task.Stages.Add(new Stage { Name = name, Weight = weight });
    }

    public void DeleteStage(Task task, Stage stage)
    {
        task.Stages.Remove(stage);
    }

    public void EditStage(Stage stage, string? name = null, int? weight = null)
    {
        if (name != null)
            stage.Name = name;
        if (weight != null)
            stage.Weight = weight.Value;
    }
}
