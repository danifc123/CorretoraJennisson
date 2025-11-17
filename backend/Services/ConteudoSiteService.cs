using CorretoraJenissonLuckwuAPI.EFModel.Entities;
using CorretoraJenissonLuckwuAPI.Repository;

namespace CorretoraJenissonLuckwuAPI.Services
{
    public class ConteudoSiteService
    {
        #region Variables
        private readonly ConteudoSiteRepository _repository;
        #endregion

        #region Constructors
        public ConteudoSiteService(ConteudoSiteRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region Methods
        public async Task<List<ConteudoSite>> GetAll(bool incluirInativos)
        {
            return await _repository.GetAllAsync(incluirInativos);
        }

        public async Task<ConteudoSite?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ConteudoSite?> GetByChave(string chave)
        {
            return await _repository.GetByChaveAsync(chave.ToLowerInvariant());
        }

        public async Task<ConteudoSite?> Add(ConteudoSite conteudo)
        {
            return await _repository.AddAsync(conteudo);
        }

        public async Task<ConteudoSite?> Update(int id, ConteudoSite conteudo)
        {
            return await _repository.UpdateAsync(id, conteudo);
        }

        public async Task<ConteudoSite?> Delete(int id)
        {
            return await _repository.DeleteAsync(id);
        }
        #endregion
    }
}

