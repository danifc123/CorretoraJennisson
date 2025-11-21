using CorretoraJenissonLuckwuAPI.EFModel.Configurations;
using CorretoraJenissonLuckwuAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CorretoraJenissonLuckwuAPI.Repository
{
    public class FavoritoRepository
    {
        private readonly CorretoraJenissonLuckwuDb _context;

        public FavoritoRepository(CorretoraJenissonLuckwuDb context)
        {
            _context = context;
        }

        public async Task<Favorito?> GetByIdAsync(int id)
        {
            return await _context.Favoritos
                .Include(f => f.Usuario)
                .Include(f => f.Imovel)
                    .ThenInclude(i => i.Imagens)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Favorito>> GetAllAsync()
        {
            return await _context.Favoritos
                .Include(f => f.Usuario)
                .Include(f => f.Imovel)
                    .ThenInclude(i => i.Imagens)
                .ToListAsync();
        }

        public async Task<List<Favorito>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Favoritos
                .Include(f => f.Imovel)
                    .ThenInclude(i => i.Imagens)
                .Where(f => f.Usuario_Id == usuarioId)
                .ToListAsync();
        }

        public async Task<List<Favorito>> GetByImovelIdAsync(int imovelId)
        {
            return await _context.Favoritos
                .Include(f => f.Usuario)
                .Where(f => f.Imovel_Id == imovelId)
                .ToListAsync();
        }

        public async Task<Favorito?> GetByUsuarioAndImovelAsync(int usuarioId, int imovelId)
        {
            return await _context.Favoritos
                .Include(f => f.Usuario)
                .Include(f => f.Imovel)
                .FirstOrDefaultAsync(f => f.Usuario_Id == usuarioId && f.Imovel_Id == imovelId);
        }

        public async Task<Favorito?> AddAsync(Favorito favorito)
        {
            // Verificar se já existe
            var existe = await GetByUsuarioAndImovelAsync(favorito.Usuario_Id, favorito.Imovel_Id);
            if (existe != null)
                return existe; // Idempotente: retorna o favorito existente

            await _context.Favoritos.AddAsync(favorito);
            await _context.SaveChangesAsync();
            return favorito;
        }

        public async Task<Favorito?> DeleteAsync(int id)
        {
            var favoritoBanco = await _context.Favoritos.FindAsync(id);

            if (favoritoBanco == null)
                return null;

            _context.Favoritos.Remove(favoritoBanco);
            await _context.SaveChangesAsync();

            return favoritoBanco;
        }

        public async Task<bool> DeleteByUsuarioAndImovelAsync(int usuarioId, int imovelId)
        {
            var favorito = await GetByUsuarioAndImovelAsync(usuarioId, imovelId);
            
            if (favorito == null)
                return false;

            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

