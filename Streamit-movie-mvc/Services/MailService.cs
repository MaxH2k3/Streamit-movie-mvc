using FluentEmail.Core;
using MailKit.Security;
using MimeKit;
using Movies.Business.users;
using Movies.Configuration;
using Movies.Models;
using Movies.Repository;
using Streamit_movie_mvc.Models.Domain;
using System.Text;

namespace Movies.Service
{
    public class MailService : IMailService
    {
        private readonly GmailConfig _gmailConfig;
        private readonly IFluentEmail _iFluentEmail;

        public MailService(GmailConfig gmailConfig, IFluentEmail iFluentEmail)
        {
            _gmailConfig = gmailConfig;
            _iFluentEmail = iFluentEmail;
        }

        public MailService(IFluentEmail iFluentEmail)
        {
            _gmailConfig = new GmailConfig();
            _iFluentEmail = iFluentEmail;
        }

        //using fluent email
        public async Task<bool> SendUsingTemplateFromFile(string template, string title, UserMail userMail)
        {
            var response = await _iFluentEmail.To(userMail.Email)
                .Subject(title)
                .UsingTemplateFromFile(template, userMail, true)
                .SendAsync();

            return response.Successful;

        }

        public MimeMessage CreateMailAsync(Mail mail, UserMail userMail)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_gmailConfig.GmailSetting.DisplayName, _gmailConfig.GmailSetting.Mail);
            email.From.Add(new MailboxAddress(_gmailConfig.GmailSetting.DisplayName, _gmailConfig.GmailSetting.Mail));
            
            email.To.Add(new MailboxAddress(mail.To, mail.To));
            email.Subject = mail.Subject;
            
            var builder = new BodyBuilder();
            //builder.HtmlBody = mail.Body;
            var htmlPart = ReadFile(mail.Body);

            //transfer data
            htmlPart = TransferData(new Dictionary<string, string>
            {
                {"username", userMail.UserName},
                {"userId", userMail.UserId},
                {"token", userMail.Token}
            }, htmlPart);

            builder.HtmlBody = htmlPart.ToString();
            email.Body = builder.ToMessageBody();

            return email;
        }

        public MimeMessage CreateMailWithAttachment(Mail mail, UserMail userMail, string attachmentFilePath)
        {
            var email = CreateMailAsync(mail, userMail);

            var builder = new BodyBuilder();

            //attachment file
            var attachment = new MimePart("application", "octet-stream")
            {
                Content = new MimeContent(File.OpenRead(attachmentFilePath)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(attachmentFilePath)
            };

            builder.Attachments.Add(attachment);
            
            email.Body = builder.ToMessageBody();

            return email;
        }

        public async Task<bool> SendMail(MimeMessage mimeMessage)
        {
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_gmailConfig.GmailSetting.SmtpServer, _gmailConfig.GmailSetting.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_gmailConfig.GmailSetting.Mail, _gmailConfig.GmailSetting.Password);
                await smtp.SendAsync(mimeMessage);

                await smtp.DisconnectAsync(true);
            } catch (System.Exception)
            {
                Console.WriteLine("Error!");
                return false;
            }

            return true;
        }

        public string ReadFile(string path)
        {
            StringBuilder html = new StringBuilder();

            using (StreamReader r = File.OpenText(path))
            {
                html.Append(r.ReadToEnd());
            }

            return html.ToString();
        }

        public string TransferData(Dictionary<string, string> models, string htmlFile)
        {
            foreach (var item in models)
            {
                htmlFile = htmlFile.Replace("{" + item.Key + "}", item.Value);
            }
            return htmlFile;
        }

    }
}
