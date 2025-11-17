using CorretoraJenissonLuckwuAPI.Models;
using CorretoraJenissonLuckwuAPI.Repository;

namespace CorretoraJenissonLuckwuAPI.Services
{
    public class AdministradorService
    {
        private AdministradorRepository _repository;
        private PasswordService _passwordService;

        public AdministradorService(AdministradorRepository repository, PasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<Administrador?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Administrador?> Add(Administrador administrador)
        {
            // Hash da senha antes de salvar
            administrador.Senha = _passwordService.HashPassword(administrador.Senha);
            return await _repository.AddAsync(administrador);
        }
        public async Task<Administrador?> GetByNome(string nome)
        {
            return await _repository.GetByNomeAsync(nome);
        }
        public async Task<Administrador?> GetByEmail(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }
        public async Task<Administrador?> Post(int id, Administrador administrador) 
        {
            // Se a senha foi fornecida, fazer hash antes de atualizar
            if (!string.IsNullOrEmpty(administrador.Senha))
            {
                administrador.Senha = _passwordService.HashPassword(administrador.Senha);
            }
            return await _repository.PostAsync(id, administrador);
        }
        public async Task<Administrador?> Delete(int id) 
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
