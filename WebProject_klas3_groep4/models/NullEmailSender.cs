using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WebProject_klas3_groep4.models
{
    public class NullEmailSender : IEmailSender<User>
    {
        public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            // Hier zou je normaal een mail sturen met bevestigingslink
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            // Hier zou je normaal een mail sturen met wachtwoord-reset link
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            // Hier zou je normaal een mail sturen met een reset-code
            return Task.CompletedTask;
        }
    }
}