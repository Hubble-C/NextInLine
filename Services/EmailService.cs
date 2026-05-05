using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using NextInLine.Settings;

namespace NextInLine.Services;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }
    
    private async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.Email));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("html") { Text = cuerpoHtml };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, false);
        await client.AuthenticateAsync(_settings.Email, _settings.Password);
        await client.SendAsync(mensaje);
        await client.DisconnectAsync(true);
    }
    
    public async Task SendTicket(string destinatario, string nombreCuenta, string code)
    {
      var message = "Tu turno ha sido creado";

      var body = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>Turno creado</title>
</head>
<body style=""margin:0; padding:0; background-color:#f4f4f5; font-family: Arial, sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding: 40px 16px;"">
    <tr>
      <td align=""center"">
        <table width=""520"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:12px; overflow:hidden;"">
          
          <!-- Header -->
          <tr>
            <td style=""background:#1a1a2e; padding:32px 32px 24px; text-align:center;"">
              <div style=""width:48px; height:48px; background:#4f46e5; border-radius:12px; margin:0 auto 16px; line-height:48px; font-size:24px;"">🎫</div>
              <p style=""color:#a5b4fc; font-size:11px; letter-spacing:2px; margin:0; text-transform:uppercase;"">Sistema de Turnos</p>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding:32px;"">
              <p style=""font-size:15px; color:#6b7280; margin:0 0 4px;"">Hola,</p>
              <p style=""font-size:20px; font-weight:600; color:#111827; margin:0 0 24px;"">{nombreCuenta}</p>

              <div style=""background:#f9fafb; border-left:3px solid #4f46e5; border-radius:0 8px 8px 0; padding:16px 20px; margin-bottom:24px;"">
                <p style=""font-size:11px; color:#6b7280; margin:0 0 4px; text-transform:uppercase; letter-spacing:1px;"">Tu turno fue creado</p>
                <p style=""font-size:28px; font-weight:700; color:#4f46e5; margin:0; letter-spacing:4px;"">{code}</p>
              </div>

              <p style=""font-size:14px; color:#6b7280; line-height:1.7; margin:0 0 24px;"">
                Por favor, mantente atento al llamado. Te notificaremos cuando sea tu turno.
              </p>

              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-top:1px solid #e5e7eb; padding-top:20px;"">
                <tr>
                  <td>
                    <p style=""font-size:11px; color:#9ca3af; margin:0 0 2px; text-transform:uppercase; letter-spacing:1px;"">Fecha</p>
                    <p style=""font-size:13px; color:#111827; margin:0; font-weight:600;"">{DateTime.Now:dd MMMM, yyyy}</p>
                  </td>
                  <td>
                    <p style=""font-size:11px; color:#9ca3af; margin:0 0 2px; text-transform:uppercase; letter-spacing:1px;"">Estado</p>
                    <p style=""font-size:13px; color:#059669; margin:0; font-weight:600;"">Activo</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background:#f9fafb; padding:16px 32px; text-align:center; border-top:1px solid #e5e7eb;"">
              <p style=""font-size:12px; color:#9ca3af; margin:0;"">Este es un mensaje automático, por favor no respondas.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        await EnviarAsync(destinatario, message , body); 
    }
}