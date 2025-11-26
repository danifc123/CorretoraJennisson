using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CorretoraJenissonLuckwuAPI.Services
{
    public class EmailService
    {
        #region Variables
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _frontendBaseUrl;
        #endregion

        #region Constructors
        public EmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var resendSection = configuration.GetSection("Resend");
            _apiKey = resendSection["ApiKey"] 
                      ?? throw new InvalidOperationException("Resend:ApiKey não configurado");
            _fromEmail = resendSection["FromEmail"] 
                         ?? throw new InvalidOperationException("Resend:FromEmail não configurado");
            _frontendBaseUrl = resendSection["FrontendBaseUrl"] 
                               ?? "http://localhost:4200";

            _httpClient.BaseAddress = new Uri("https://api.resend.com/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }
        #endregion

        #region Methods
        public async Task SendPasswordResetEmailAsync(string email, string? nome, string token, string role)
        {
            var nomeFinal = string.IsNullOrWhiteSpace(nome) ? "Olá" : nome;
            var resetLink = $"{_frontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(role)}";

            var subject = "Recuperação de senha - Jenisson Luckwü Imóveis";

            var html = new StringBuilder();
            html.Append($@"
                <p>{nomeFinal},</p>
                <p>Recebemos uma solicitação para redefinir a senha da sua conta na <strong>Jenisson Luckwü Imóveis</strong>.</p>
                <p>Clique no botão abaixo para criar uma nova senha:</p>
                <p>
                    <a href=""{resetLink}"" 
                       style=""display:inline-block;padding:10px 18px;background-color:#1E3A8A;color:#ffffff;
                              text-decoration:none;border-radius:4px;font-weight:bold;"">
                        Redefinir minha senha
                    </a>
                </p>
                <p>Se o botão não funcionar, copie e cole o link abaixo no seu navegador:</p>
                <p><a href=""{resetLink}"">{resetLink}</a></p>
                <p><strong>Importante:</strong> este link é válido por 24 horas. Após esse período, será necessário solicitar um novo link.</p>
                <p>Se você não fez esta solicitação, pode ignorar este e-mail com segurança.</p>
                <p>Atenciosamente,<br/>Equipe Jenisson Luckwü Imóveis</p>
            ");

            var payload = new
            {
                from = _fromEmail,
                to = new[] { email },
                subject,
                html = html.ToString()
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("emails", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Erro ao enviar e-mail de reset de senha via Resend: {response.StatusCode} - {body}");
            }
        }
        #endregion
    }
}


