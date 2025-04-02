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
        /// <exception cref="SmtpException">Thrown if there is an error sending the email via SMTP.</exception>
        public static bool SendEmailTwoFactorCode(string? userEmail, string code)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new InvalidOperationException($"{nameof(userEmail)} is null or empty.");
            }

            // Create the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress("imperialtuho-admin@test.com"),
                Subject = "Two Factor Code",
                IsBodyHtml = true,
                Body = code
            };

            // Add the recipient email address
            mailMessage.To.Add(new MailAddress(userEmail));

            // Configure the SMTP client
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("b3763ec6ff4b9d", "0e581a05dbec85"),
                EnableSsl = true
            };

            try
            {
                // Send the email
                client.Send(mailMessage);
                return true; // Return true if the email was sent successfully
            }
            catch (SmtpException ex)
            {
                // Log the SMTP exception details
                Console.WriteLine($"SMTP error: {ex.Message}");
                return false; // Return false if there was an error sending the email
            }
            catch (Exception ex)
            {
                // Log any other exceptions
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false; // Return false for unexpected errors
            }
        }
    }
}