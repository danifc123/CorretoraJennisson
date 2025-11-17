using System;
using System.Linq;
using CorretoraJenissonLuckwuAPI.EFModel.Configurations;
using CorretoraJenissonLuckwuAPI.EFModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace CorretoraJenissonLuckwuAPI.Repository
{
    public class ConteudoSiteRepository
    {
        #region Variables
        private readonly CorretoraJenissonLuckwuDb _context;
        #endregion

        #region Constructors
        public ConteudoSiteRepository(CorretoraJenissonLuckwuDb context)
        {
            _context = context;
        }
        #endregion

        #region Methods
        public async Task<List<ConteudoSite>> GetAllAsync(bool incluirInativos)
        {
            var query = _context.ConteudosSite.AsQueryable();
            if (!incluirInativos)
            {
                query = query.Where(c => c.Ativo);
            }
            return await query
                .OrderBy(c => c.Ordem)
                .ThenByDescending(c => c.AtualizadoEm)
                .ToListAsync();
        }

        public async Task<ConteudoSite?> GetByIdAsync(int id)
        {
            return await _context.ConteudosSite.FindAsync(id);
        }

        public async Task<ConteudoSite?> GetByChaveAsync(string chave)
        {
            return await _context.ConteudosSite
                .FirstOrDefaultAsync(c => c.Chave == chave);
        }

        public async Task<ConteudoSite?> AddAsync(ConteudoSite conteudo)
        {
            conteudo.Chave = conteudo.Chave.ToLowerInvariant();
            conteudo.AtualizadoEm = DateTime.UtcNow;
            conteudo.CriadoEm = DateTime.UtcNow;

            await _context.ConteudosSite.AddAsync(conteudo);
            await _context.SaveChangesAsync();
            return conteudo;
        }

        public async Task<ConteudoSite?> UpdateAsync(int id, ConteudoSite conteudo)
        {
            var conteudoBanco = await _context.ConteudosSite.FindAsync(id);
            if (conteudoBanco == null)
                return null;

            conteudoBanco.Titulo = conteudo.Titulo;
            conteudoBanco.Subtitulo = conteudo.Subtitulo;
            conteudoBanco.Descricao = conteudo.Descricao;
            conteudoBanco.ImagemUrl = conteudo.ImagemUrl;
            conteudoBanco.Ordem = conteudo.Ordem;
            conteudoBanco.Ativo = conteudo.Ativo;
            conteudoBanco.Chave = conteudo.Chave.ToLowerInvariant();
            conteudoBanco.AtualizadoEm = DateTime.UtcNow;

            _context.ConteudosSite.Update(conteudoBanco);
            await _context.SaveChangesAsync();
            return conteudoBanco;
        }

        public async Task<ConteudoSite?> DeleteAsync(int id)
        {
            var conteudoBanco = await _context.ConteudosSite.FindAsync(id);
            if (conteudoBanco == null)
                return null;

            _context.ConteudosSite.Remove(conteudoBanco);
            await _context.SaveChangesAsync();
            return conteudoBanco;
        }
        #endregion
    }
}

