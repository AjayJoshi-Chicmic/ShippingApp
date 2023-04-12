using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IEmailService
    {
        public ResponseModel SendEmail(EmailModel email);
    }
}
