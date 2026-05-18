namespace TimeAccountingSystem;

public class Employee: Staff
{
    // Конструктор (зсилається на батьківські класи)
    public Employee(string name, string id, decimal salary, decimal bonus)
        : base(name, id, salary, bonus)
    {
    }

    public void CompleteStage(Stage stage)
    {
        stage.EndTime = DateTime.Now;
    }
}
