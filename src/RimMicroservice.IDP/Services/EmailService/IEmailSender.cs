namespace RimMicroservices.IDP.Services.EmailService
{
    public interface IEmailSender
    {
        void SendEmail(string recipient, string subject, string body, bool isBodyHTML = false, string sender = null);
    }
}
