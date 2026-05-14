namespace TimeAccountingSystem;

public abstract class User
{
	// Характеристики користувача
	protected string name;
	protected string id;
	protected bool isLoggedIn = false;

    // Метод для створення коментаря до завдання (список і структура в Task.cs)
    public void CreateComment(string message, Task task)
	{
		task.Comments.Add(new Task.Comment { Message = message, Author = this });
    }

    // Метод для авторизації користувача
    public void LogIn(string name, string id)
	{
		if (this.id == id && this.name == name)
			isLoggedIn = true;
		else
			throw new Exception("Invalid credentials");
    }

    // Метод для виходу користувача
    public void LogOut()
	{
		isLoggedIn = false;
    }

	// Метод буде наслідуватись і доповнюватись
	public void ShowInfo()
	{
		Console.Write($"Ім'я: {this.name}, ID: {this.id}");
    }
}
