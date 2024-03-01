using Movies.Configuration;

namespace Streamit_movie_mvc.Services.Configuration;

public class GmailSetting
{
    public string DisplayName { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }

    public GmailSetting()
    {
        IConfiguration config = new ConfigurationBuilder()

                .SetBasePath(Directory.GetCurrentDirectory())

                .AddJsonFile("appsettings.json", true, true)

                .Build();

        config.GetSection("GmailSetting");

        DisplayName = config["GmailSetting:DisplayName"];
        SmtpServer = config["GmailSetting:SmtpServer"];
        Port = int.Parse(config["GmailSetting:Port"]);
        Mail = config["GmailSetting:Mail"];
        Password = config["GmailSetting:Password"];
    }

    public override string? ToString()
    {
        return "DisplayName: " + DisplayName + "\n" +
               "SmtpServer: " + SmtpServer + "\n" +
               "Port: " + Port + "\n" +
               "Mail: " + Mail + "\n" +
               "Password: " + Password + "\n";
    }
}

public static class FluentEmailExtensions
{
    public static void AddFluentEmail(this IServiceCollection services)
    {
        GmailSetting gmailSetting = new GmailSetting();

        services.AddFluentEmail(gmailSetting.Mail)
        .AddSmtpSender(gmailSetting.SmtpServer, gmailSetting.Port,
                        gmailSetting.DisplayName, gmailSetting.Password)
        .AddRazorRenderer();
    }
}
