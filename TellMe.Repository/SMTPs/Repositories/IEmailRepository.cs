﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.SMTPs.Repositories
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(MailMessage mailMessage);
    }
}
