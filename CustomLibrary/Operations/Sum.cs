namespace CustomLibrary.Operations
{
    public class Sum : IMath
    {
        public decimal Operation(decimal a, decimal b)
        {
            return a + b;
        }
    }
}
