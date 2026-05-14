namespace TimeAccountingSystem;

public abstract class Staff: User
{
    // Характеристики менеджера та робітника
    private decimal salary;
    private decimal bonus;

    // Властивості (додайте валідацію, якщо хочете)
    public decimal Salary
    {
        get { return salary; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            salary = value;
        }
    }

    public decimal Bonus
    {
        get { return bonus; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            bonus = value;
        }
    }

    // Конструктор (зсилається на батьківський клас)
    public Staff(string name, string id, decimal salary, decimal bonus)
        : base(name, id)
    {
        this.Salary = salary;
        this.Bonus = bonus;
    }
}
