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
                    throw new FileNotFoundException(MessageConstant.Email.TemplateNotFound, templateFilePath);
                }

                var body = await File.ReadAllTextAsync(templateFilePath);
                body = body.Replace("@Model.FullName", request.FullName)
                      .Replace("@Model.ConfirmationLink", request.ConfirmationLink);

                var mailMessage = new MailMessage
                {
                    Subject = MessageConstant.Email.ConfirmSubject,
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                };
                mailMessage.To.Add(new MailAddress(request.Email, request.FullName));

                // Tạo một đối tượng AlternateView cho HTML content
                var htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");

                // Thêm hình ảnh (embedded image)
                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstant.PathAssets.TellMe); // Đường dẫn đến hình ảnh
                if (File.Exists(imagePath))
                {
                    var logo = new LinkedResource(imagePath, "image/jpg")
                    {
                        ContentId = "Logo_TellMe",
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };
                    htmlView.LinkedResources.Add(logo);
                }
                else
                {
                    throw new FileNotFoundException(MessageConstant.Email.LogoImageNotFound, imagePath);
                }

                mailMessage.AlternateViews.Add(htmlView);

                await _emailRepository.SendEmailAsync(mailMessage);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task SendForgotPasswordEmailAsync(ForgotPasswordEmailRequest request)
        {
            try
            {
                var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstant.PathTemplate.ForgotPassword);

                if (!File.Exists(templateFilePath))
                {
                    throw new FileNotFoundException(MessageConstant.Email.TemplateNotFound, templateFilePath);
                }

                var body = await File.ReadAllTextAsync(templateFilePath);
                body = body.Replace("@Model.FullName", request.FullName)
                          .Replace("@Model.ResetPasswordLink", request.ResetPasswordLink);

                var mailMessage = new MailMessage
                {
                    Subject = MessageConstant.Email.ForgotPasswordSubject,
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                };
                mailMessage.To.Add(new MailAddress(request.Email, request.FullName));

                // Tạo một đối tượng AlternateView cho HTML content
                var htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");

                // Thêm hình ảnh (embedded image)
                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathConstant.PathAssets.TellMe); // Đường dẫn đến hình ảnh
                if (File.Exists(imagePath))
                {
                    var logo = new LinkedResource(imagePath, "image/jpg")
                    {
                        ContentId = "Logo_TellMe",
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };
                    htmlView.LinkedResources.Add(logo);
                }
                else
                {
                    throw new FileNotFoundException(MessageConstant.Email.LogoImageNotFound, imagePath);
                }

                mailMessage.AlternateViews.Add(htmlView);

                await _emailRepository.SendEmailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
