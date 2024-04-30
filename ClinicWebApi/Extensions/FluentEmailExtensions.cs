using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using System.Net;
using System.Net.Mail;

namespace ClinicWebApi.Extensions
{
    public static class FluentEmailExtensions
    {
        public static void AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            var defaultFromEmail = emailSettings["DefaultFromEmail"];
            var host = emailSettings["Host"];
            var port = emailSettings.GetValue<int>("Port");
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            var client = new SmtpClient(host, port);
            client.Credentials = new NetworkCredential(username, password);
            /*            services.AddSingleton<ISender>(x => new SmtpSender(new SmtpClient(host, port)));
             
                .AddSmtpSender(client);
            services.AddFluentEmail(defaultFromEmail);
             */

            services
                .AddFluentEmail(defaultFromEmail);

            services.AddSingleton<ISender>(x => new SmtpSender(client));
        }
    }
}
