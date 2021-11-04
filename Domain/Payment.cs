using LanguageExt;
using static LanguageExt.Prelude;


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
        public static bool ValidateInputPayment(string paymentinfo, out Payment payment)
        {
            bool isValid = false;
            payment = null;
            if (paymentinfo.Equals("y") || paymentinfo.Equals("n"))
            {
                isValid = true;
                payment = new(paymentinfo);
            }
            return isValid;
        }

        public static Option<Payment> TryParsePayment(string payment)
        {
            if (payment.Equals("y") || payment.Equals("n"))
            {
                return Some<Payment>(new(payment));
            }
            else
            {
                return None;
            }
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }
    }
}