using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Evertech.Overtime.Domain.Helpers;

internal sealed class EmailHelper
{
    private string title;
    private List<string> recipients;
    private List<string> recipientsCopy;
    private List<string> recipientsCopyHidden;
    private string body;
    private readonly List<(MemoryStream, string, string)> attachments;
    private readonly SmtpClient smtpClient;
    private readonly string fromEmail;

    private EmailHelper(IConfiguration configuration)
    {
        fromEmail = configuration.GetSection("EmailUser").Value;
        smtpClient = new SmtpClient(configuration.GetSection("EmailHost").Value)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = bool.Parse(configuration.GetSection("EmailEnableSSL").Value),
            UseDefaultCredentials = bool.Parse(configuration.GetSection("EmailUseDefaultCredentials").Value),
            Credentials = new NetworkCredential(fromEmail, configuration.GetSection("EmailPassword").Value),
            Port = int.Parse(configuration.GetSection("EmailPort").Value)
        };
        attachments = [];
    }

    public static EmailHelper Create(IConfiguration configuration) => new(configuration);

    public EmailHelper WithTitle(string title)
    {
        this.title = title;
        return this;
    }

    public EmailHelper WithRecipient(string recipient)
    {
        recipients = [recipient];
        return this;
    }

    public EmailHelper WithRecipients(IList<string> recipients)
    {
        this.recipients = [.. recipients];
        return this;
    }

    public EmailHelper WithRecipientCopy(string recipientCopy)
    {
        recipientsCopy = [recipientCopy];
        return this;
    }

    public EmailHelper WithRecipientsCopy(IList<string> recipientsCopy)
    {
        this.recipientsCopy = [.. recipientsCopy];
        return this;
    }

    public EmailHelper WithRecipientCopyHidden(string recipientCopyHidden)
    {
        recipientsCopyHidden = [recipientCopyHidden];
        return this;
    }

    public EmailHelper WithRecipientsCopyHidden(IList<string> recipientsCopyHidden)
    {
        this.recipientsCopyHidden = [.. recipientsCopyHidden];
        return this;
    }

    public EmailHelper WithBody(string body)
    {
        this.body = body;
        return this;
    }

    public EmailHelper AddAttachment(byte[] memoryFile, string nameAttachment, string cid = null)
    {
        attachments.Add((new MemoryStream(memoryFile), nameAttachment, cid));
        return this;
    }

    private MailMessage GetMailMessage()
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentNullException("Title is not defined in e-mail");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentNullException("Body is not defined in e-mail");
        }

        if (recipients == null || recipients.Count == 0)
        {
            throw new ArgumentNullException("Recipients is not defined in e-mail");
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = title,
            Body = body,
            IsBodyHtml = true
        };

        foreach (string to in recipients)
        {
            mailMessage.To.Add(to);
        }

        if (recipientsCopy != null)
        {
            foreach (string cc in recipientsCopy)
            {
                mailMessage.CC.Add(cc);
            }
        }

        if (recipientsCopyHidden != null)
        {
            foreach (string bcc in recipientsCopyHidden)
            {
                mailMessage.Bcc.Add(bcc);
            }
        }

        foreach (var item in attachments)
        {
            var ath = new Attachment(item.Item1, item.Item2);
            if (item.Item3 != null)
            {
                ath.ContentDisposition!.Inline = true;
                ath.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                ath.ContentId = item.Item3;
                ath.ContentType.MediaType = "image/png";
                ath.ContentType.Name = "png";
            }
            mailMessage.Attachments.Add(ath);
        }

        return mailMessage;
    }

    public bool SendWait()
    {
        var mailMessage = GetMailMessage();

        try
        {
            smtpClient.Send(mailMessage);
            return true;
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Failed send e-mail. '{exc.Message}'");
            return false;
        }
    }

    public void Send()
    {
        var mailMessage = GetMailMessage();
        var tuple = new Tuple<MailMessage, SmtpClient>(mailMessage, smtpClient);
        var thread = new Thread(new ParameterizedThreadStart(InternalSend));
        thread.Start(tuple);
    }

    private static void InternalSend(object obj)
    {
        try
        {
            var tuple = obj as Tuple<MailMessage, SmtpClient>;
            tuple.Item2.Send(tuple.Item1);
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Failed send e-mail. '{exc.Message}'");
        }
    }
}