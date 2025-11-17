using CorretoraJenissonLuckwuAPI.EFModel.Entities;
using CorretoraJenissonLuckwuAPI.Repository;

namespace CorretoraJenissonLuckwuAPI.Services
{
    public class ImagemImovelService
    {
        private readonly ImagemImovelRepository _repository;

        public ImagemImovelService(ImagemImovelRepository repository)
        {
            _repository = repository;
        }

        public async Task<ImagemImovel?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<ImagemImovel>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<ImagemImovel>> GetByImovelId(int imovelId)
        {
            return await _repository.GetByImovelIdAsync(imovelId);
        }

        public async Task<ImagemImovel?> Add(ImagemImovel imagemImovel)
        {
            return await _repository.AddAsync(imagemImovel);
        }

        public async Task<ImagemImovel?> Update(int id, ImagemImovel imagemImovel)
        {
            return await _repository.UpdateAsync(id, imagemImovel);
        }

        public async Task<ImagemImovel?> Delete(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteByImovelId(int imovelId)
        {
            return await _repository.DeleteByImovelIdAsync(imovelId);
        }
    }
}

