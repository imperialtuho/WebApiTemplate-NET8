using System.Net;
using System.Net.Mail;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides helper methods for sending emails, including two-factor authentication codes.
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Sends a two-factor authentication code to the specified email address.
        /// </summary>
        /// <param name="userEmail">The recipient's email address.</param>
        /// <param name="code">The two-factor authentication code to send.</param>
        /// <returns>True if the email was sent successfully; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the email address is null or empty.</exception>

        public static bool SendEmailTwoFactorCode(string? userEmail, string code)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new InvalidOperationException($"{nameof(userEmail)} is null or empty.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress("imperialtuho-admin@test.com")
            };

            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Two Factor Code";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = code;

            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("b3763ec6ff4b9d", "0e581a05dbec85"),
                EnableSsl = true
            };
            client.Credentials = new NetworkCredential("b3763ec6ff4b9d", "0e581a05dbec85");

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}