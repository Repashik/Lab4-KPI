namespace TimeAccountingSystem;

public class Admin: User
{
    // Конструктор (зсилається на батьківський клас)
    public Admin(string name, string id)
        : base(name, id)
    {
    }

    // Метод для створення нового користувача
    public void Create(Role role, string name, string id, decimal salary, decimal bonus, List<User> users)
    {
        User newUser;
        switch (role)
        {
            case Role.Manager:
                newUser = new Manager(name, id, salary, bonus);
                break;
            case Role.Employee:
                newUser = new Employee(name, id, salary, bonus);
                break;
            case Role.Admin:
                newUser = new Admin(name, id);
                break;
            default:
                throw new Exception("Invalid role");
        }

        users.Add(newUser);
    }

    // Метод для видалення користувача
    public void Delete(string id, List<User> users)
    {
        foreach (User user in users)
        {
            if (user.Id == id)
            {
                users.Remove(user);
            }
        }
    }
}
