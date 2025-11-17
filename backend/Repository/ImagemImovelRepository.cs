using CorretoraJenissonLuckwuAPI.EFModel.Configurations;
using CorretoraJenissonLuckwuAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CorretoraJenissonLuckwuAPI.Repository
{
    public class ImagemImovelRepository
    {
        private readonly CorretoraJenissonLuckwuDb _context;

        public ImagemImovelRepository(CorretoraJenissonLuckwuDb context)
        {
            _context = context;
        }

        public async Task<ImagemImovel?> GetByIdAsync(int id)
        {
            return await _context.ImagemImoveis
                .Include(i => i.Imovel)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<ImagemImovel>> GetAllAsync()
        {
            return await _context.ImagemImoveis
                .Include(i => i.Imovel)
                .ToListAsync();
        }

        public async Task<List<ImagemImovel>> GetByImovelIdAsync(int imovelId)
        {
            return await _context.ImagemImoveis
                .Where(i => i.Imovel_Id == imovelId)
                .ToListAsync();
        }

        public async Task<ImagemImovel?> AddAsync(ImagemImovel imagemImovel)
        {
            await _context.ImagemImoveis.AddAsync(imagemImovel);
            await _context.SaveChangesAsync();
            return imagemImovel;
        }

        public async Task<ImagemImovel?> UpdateAsync(int id, ImagemImovel imagemImovel)
        {
            var imagemBanco = await _context.ImagemImoveis.FindAsync(id);

            if (imagemBanco == null)
                return null;

            imagemBanco.Url = imagemImovel.Url;
            imagemBanco.Imovel_Id = imagemImovel.Imovel_Id;

            _context.ImagemImoveis.Update(imagemBanco);
            await _context.SaveChangesAsync();

            return imagemBanco;
        }

        public async Task<ImagemImovel?> DeleteAsync(int id)
        {
            var imagemBanco = await _context.ImagemImoveis.FindAsync(id);

            if (imagemBanco == null)
                return null;

            _context.ImagemImoveis.Remove(imagemBanco);
            await _context.SaveChangesAsync();

            return imagemBanco;
        }

        public async Task<bool> DeleteByImovelIdAsync(int imovelId)
        {
            var imagens = await _context.ImagemImoveis
                .Where(i => i.Imovel_Id == imovelId)
                .ToListAsync();

            if (imagens.Any())
            {
                _context.ImagemImoveis.RemoveRange(imagens);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}

