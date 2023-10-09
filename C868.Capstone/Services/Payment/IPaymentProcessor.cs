namespace C868.Capstone.Services.Payment
{
    public interface IPaymentProcessor
    {
        bool ProcessPayment(double total);
    }
}