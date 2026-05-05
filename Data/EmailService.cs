using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Team_Aura_Period_Tracker_;

public class EmailService
{
    private const string SenderEmail = "teamauraofficial94@gmail.com";
    private const string SenderAppPassword = "pbwzyzwygscxgtsa";

    public async Task SendOtpEmailAsync(string receiverEmail, string otp)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Team Aura", SenderEmail));
        message.To.Add(MailboxAddress.Parse(receiverEmail));
        message.Subject = "Your Password Reset OTP";

        message.Body = new TextPart("html")
        {
            Text = $@"
            <div style='font-family: Arial; padding: 20px;'>
                <h2 style='color:#E85B73;'>Team Aura Password Reset</h2>
                <p>Your OTP code is:</p>
                <h1 style='color:#E85B73; letter-spacing: 4px;'>{otp}</h1>
                <p>Please use this code to reset your password.</p>
            </div>"
        };

        using var client = new SmtpClient();

        client.ServerCertificateValidationCallback =
            (sender, certificate, chain, sslPolicyErrors) => true;

        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(SenderEmail, SenderAppPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}