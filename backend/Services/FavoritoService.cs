using CorretoraJenissonLuckwuAPI.EFModel.Entities;
using CorretoraJenissonLuckwuAPI.Repository;

namespace CorretoraJenissonLuckwuAPI.Services
{
    public class FavoritoService
    {
        private readonly FavoritoRepository _repository;

        public FavoritoService(FavoritoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Favorito?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Favorito>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Favorito>> GetByUsuarioId(int usuarioId)
        {
            return await _repository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task<List<Favorito>> GetByImovelId(int imovelId)
        {
            return await _repository.GetByImovelIdAsync(imovelId);
        }

        public async Task<Favorito?> GetByUsuarioAndImovel(int usuarioId, int imovelId)
        {
            return await _repository.GetByUsuarioAndImovelAsync(usuarioId, imovelId);
        }

        public async Task<Favorito?> Add(Favorito favorito)
        {
            return await _repository.AddAsync(favorito);
        }

        public async Task<Favorito?> Delete(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteByUsuarioAndImovel(int usuarioId, int imovelId)
        {
            return await _repository.DeleteByUsuarioAndImovelAsync(usuarioId, imovelId);
        }
    }
}

