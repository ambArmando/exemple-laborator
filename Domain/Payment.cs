namespace lab1PSSC.Domain
{
    public record Payment
    {
        public string Value { get; }

        public Payment(string state)
        {
            if (state.Equals("y") || state.Equals("n"))
            {
                Value = state;
            }
            else
            {
                throw new InvalidPaymentException($"{state:0.##} is an invalid paymnet value.");
            }
        }
        public override string ToString()
        {
            return $"{Value:0.##}";
        }
    }
}