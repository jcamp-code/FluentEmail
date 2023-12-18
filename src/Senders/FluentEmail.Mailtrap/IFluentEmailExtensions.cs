using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailtrap;

namespace FluentEmail.Mailtrap
{
    public static class IFluentEmailExtensions
    {
        public static async Task<SendResponse> SendWithTemplateAsync(this IFluentEmail email, string templateName, object templateData)
        {
            var mailtrapSender = email.Sender as IMailtrapSender;
            return await mailtrapSender.SendWithTemplateAsync(email, templateName, templateData);
        }
    }
}
