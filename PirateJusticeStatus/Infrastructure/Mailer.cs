using System;
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;
using MimeKit;

namespace PirateJusticeStatus.Infrastructure
{
    public class Mailer
    {
        private const string ErrorSubject = "Error in O2A";
        private const string WarningSubject = "Warning in O2A";

        private Logger _log;
        private Config _config;

        public Mailer(Logger log, Config config)
        {
            _log = log;
            _config = config;
        }

        public void SendError(Exception exception)
        {
            SendAdmin(ErrorSubject, exception.ToString());
        }

        public void SendWarning(string body)
        {
            SendAdmin(WarningSubject, body);
        }

        public void SendAdmin(string subject, string body)
        {
            Send(_config.AdminMailAddress, _config.AdminMailAddress, subject, body);
        }

        public void Send(string toName, string toAddress, string subject, string body)
        {
            _log.Notice("Sending message to {0}", toAddress);

            try
            {
                _log.Notice("A");
                var client = new SmtpClient();
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.Connect(_config.MailServerHost, _config.MailServerPort);
				client.Authenticate(_config.MailAccountName, _config.MailAccountPassword);
                _log.Verbose("Connected to mail server {0}:{1}", _config.MailServerHost, _config.MailServerPort);
                _log.Notice("B");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config.SystemMailName, _config.SystemMailAddress));
                message.To.Add(new MailboxAddress(toName, toAddress));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };
                client.Send(message);

                _log.Notice("C");
                _log.Info("Message sent to {0}", toAddress);
            }
            catch (Exception exception)
            {
                _log.Notice("D");
                _log.Error("Error sending mail to {0}", toAddress);
                _log.Error(exception.ToString());
            }

            _log.Notice("Z");
        }
    }
}
