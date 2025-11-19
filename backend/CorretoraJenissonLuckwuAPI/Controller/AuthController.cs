using CorretoraJenissonLuckwuAPI.Models.DTOs;
using CorretoraJenissonLuckwuAPI.Repository;
using CorretoraJenissonLuckwuAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CorretoraJenissonLuckwuAPI.Controller
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    #region Variables
    private readonly AdministradorRepository _administradorRepository;
    private readonly UsuarioRepository _usuarioRepository;
    private readonly PasswordService _passwordService;
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;
    #endregion

    #region Constructors
    public AuthController(
        AdministradorRepository administradorRepository,
        UsuarioRepository usuarioRepository,
        PasswordService passwordService,
        AuthService authService,
        IConfiguration configuration)
    {
      _administradorRepository = administradorRepository;
      _usuarioRepository = usuarioRepository;
      _passwordService = passwordService;
      _authService = authService;
      _configuration = configuration;
    }
    #endregion

    #region Controllers
    [HttpPost("login-administrador")]
    public async Task<ActionResult<LoginResponse>> LoginAdministrador(LoginRequest request)
    {
      try
      {
        // Tenta autenticar como root admin primeiro
        if (TryAuthenticateRootAdmin(request, out var rootResponse))
        {
          return Ok(rootResponse);
        }

        // Busca administrador no banco de dados
        var administrador = await _administradorRepository.GetByEmailAsync(request.Email);

        if (administrador == null)
        {
          return Unauthorized("Email ou senha inválidos");
        }

        // Verifica a senha
        var isPasswordValid = _passwordService.VerifyPassword(request.Senha, administrador.Senha);
        
        if (!isPasswordValid)
        {
          return Unauthorized("Email ou senha inválidos");
        }

        return Ok(BuildLoginResponse(administrador.Id, administrador.Email, "Admin"));
      }
      catch (Exception ex)
      {
        // Log do erro para debug (em produção, usar um logger apropriado)
        Console.WriteLine($"Erro no login administrador: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
      }
    }

    [HttpPost("login-usuario")]
    public async Task<ActionResult<LoginResponse>> LoginUsuario(LoginRequest request)
    {
      try
      {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (usuario == null)
          return Unauthorized("Email ou senha inválidos");

        if (!_passwordService.VerifyPassword(request.Senha, usuario.Senha))
          return Unauthorized("Email ou senha inválidos");

        return Ok(BuildLoginResponse(usuario.Id, usuario.Email, "User"));
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
      }
    }
    [HttpPost("refresh-token")]
    public Task<ActionResult<LoginResponse>> RefreshToken(RefreshTokenRequest request)
    {
      return Task.FromResult<ActionResult<LoginResponse>>(BadRequest("Refresh token não implementado completamente. Use login novamente."));
    }

    /// <summary>
    /// Identifica o tipo de usuário (Admin ou User) baseado no email
    /// </summary>
    [HttpGet("identify-user-type")]
    public async Task<ActionResult<UserTypeResponse>> IdentifyUserType([FromQuery] string email)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(email))
        {
          return BadRequest("Email é obrigatório");
        }

        // Verifica se é root admin primeiro
        var rootSection = _configuration.GetSection("RootAdmin");
        var rootEnabled = rootSection.GetValue<bool>("Enabled");
        var rootEmail = rootSection["Email"];

        if (rootEnabled && !string.IsNullOrWhiteSpace(rootEmail) &&
            string.Equals(email, rootEmail, StringComparison.OrdinalIgnoreCase))
        {
          return Ok(new UserTypeResponse { UserType = "Admin", Exists = true });
        }

        // Verifica se existe na tabela de administradores
        var administrador = await _administradorRepository.GetByEmailAsync(email);
        if (administrador != null)
        {
          return Ok(new UserTypeResponse { UserType = "Admin", Exists = true });
        }

        // Verifica se existe na tabela de usuários
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario != null)
        {
          return Ok(new UserTypeResponse { UserType = "User", Exists = true });
        }

        // Email não encontrado em nenhuma tabela
        return Ok(new UserTypeResponse { UserType = "", Exists = false });
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
      }
    }
    #endregion

    #region Private Methods
    private bool TryAuthenticateRootAdmin(LoginRequest request, out LoginResponse response)
    {
      response = null!;

      var rootSection = _configuration.GetSection("RootAdmin");
      var enabled = rootSection.GetValue<bool>("Enabled");
      var email = rootSection["Email"];
      var password = rootSection["Password"];

      if (!enabled || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        return false;

      if (!string.Equals(request.Email, email, StringComparison.OrdinalIgnoreCase))
        return false;

      if (request.Senha != password)
        return false;

      response = BuildLoginResponse(0, email, "Admin");
      return true;
    }

    private LoginResponse BuildLoginResponse(int userId, string email, string role)
    {
      var minutes = GetAccessTokenExpirationMinutes();
      var accessToken = _authService.GenerateAccessToken(userId, email, role);
      var refreshToken = _authService.GenerateRefreshToken();

      return new LoginResponse
      {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        ExpiresAt = DateTime.UtcNow.AddMinutes(minutes),
        UserId = userId,
        Email = email,
        Role = role
      };
    }

    private double GetAccessTokenExpirationMinutes()
    {
      var jwtSection = _configuration.GetSection("JwtSettings");
      return double.TryParse(jwtSection["AccessTokenExpirationMinutes"], out var minutes)
          ? minutes
          : 15;
    }
    #endregion
  }
}

