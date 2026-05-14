namespace TimeAccountingSystem;

public class Task: WorkBase
{
    // Список коментарів до завдання
    public List<Comment> Comments = new();
    // Структура коментаря
    public struct Comment
    {
        public string Message;
        public User Author;
    }
}
