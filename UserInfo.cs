namespace TimeAccountingSystem;

public class UserInfo
{
    // Робоча інформація про користувача
    public int tasksDoneCount;
    public List<Task> tasks = new();

    // Конструктор
    public UserInfo()
    {
        tasksDoneCount = 0;
    }
}
