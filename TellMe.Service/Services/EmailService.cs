using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Infrastructures;
using TellMe.Repository.SMTPs.Repositories;
using TellMe.Service.Constants;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository) 
        { 
            _emailRepository = emailRepository;
        }

        public async Task SendConfirmationEmailAsync(ConfirmEmailRequest request)
        {
            try
            {
                var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstant.PathTemplate.ConfirmEmail);

                if (!File.Exists(templateFilePath))
                {
                    throw new FileNotFoundException(MessageConstant.EmailMessage.FileNotFound, templateFilePath);
                }

                var body = await File.ReadAllTextAsync(templateFilePath);
                body = body.Replace("@Model.FullName", request.FullName)
                      .Replace("@Model.ConfirmationLink", request.ConfirmationLink);

                var mailMessage = new MailMessage
                {
                    Subject = MessageConstant.EmailMessage.ConfirmSubject,
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                };
                mailMessage.To.Add(new MailAddress(request.Email, request.FullName));

                await _emailRepository.SendEmailAsync(mailMessage);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
