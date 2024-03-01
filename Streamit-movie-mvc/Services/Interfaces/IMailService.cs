using MimeKit;
using Movies.Business.users;
using Movies.Models;
using Streamit_movie_mvc.Models.Domain;

namespace Movies.Repository;

public interface IMailService
{
    Task<bool> SendMail(MimeMessage mimeMessage);
    MimeMessage CreateMailAsync(Mail mail, UserMail userMail);
    MimeMessage CreateMailWithAttachment(Mail mail, UserMail userMail, string attachmentFilePath);
    string TransferData(Dictionary<string, string> models, string htmlFile);
    Task<bool> SendUsingTemplateFromFile(string template, string title, UserMail userMail);
}
