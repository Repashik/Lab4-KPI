namespace TimeAccountingSystem;

public class Stage: WorkBase
{
    private int weight;
    public int Weight
    {
        get => weight;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
            weight = value;
        }
    }
}
