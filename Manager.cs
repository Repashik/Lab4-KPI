namespace TimeAccountingSystem;

public class Manager: Staff
{
    // Конструктор (зсилається на батьківські класи)
    public Manager(string name, string id, decimal salary, decimal bonus)
    : base(name, id, salary, bonus)
    {
    }
}
