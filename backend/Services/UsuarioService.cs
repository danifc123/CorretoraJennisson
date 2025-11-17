using CorretoraJenissonLuckwuAPI.Models.Entities;
using CorretoraJenissonLuckwuAPI.Repository;

namespace CorretoraJenissonLuckwuAPI.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repository;
        private readonly PasswordService _passwordService;

        public UsuarioService(UsuarioRepository repository, PasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<Usuario?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Usuario?> Add(Usuario usuario)
        {
            // Hash da senha antes de salvar
            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuario.Senha = _passwordService.HashPassword(usuario.Senha);
            }
            return await _repository.AddAsync(usuario);
        }

        public async Task<Usuario?> Update(int id, Usuario usuario)
        {
            // Se a senha foi fornecida, fazer hash antes de atualizar
            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                usuario.Senha = _passwordService.HashPassword(usuario.Senha);
            }
            return await _repository.UpdateAsync(id, usuario);
        }

        public async Task<Usuario?> Delete(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<Usuario?> GetByEmail(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }

        public async Task<Usuario?> GetByStreamUserId(string streamUserId)
        {
            return await _repository.GetByStreamUserIdAsync(streamUserId);
        }
    }
}

