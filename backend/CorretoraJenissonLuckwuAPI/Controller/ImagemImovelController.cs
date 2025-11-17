using System.Linq;
using CorretoraJenissonLuckwuAPI.Models.Entities;
using CorretoraJenissonLuckwuAPI.Infrastructure;
using CorretoraJenissonLuckwuAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CorretoraJenissonLuckwuAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagemImovelController : ControllerBase
    {
        #region Variables
        private readonly ImagemImovelService _services;
        private readonly FileStorageService _fileStorageService;
        #endregion

        #region Constructors
        public ImagemImovelController(ImagemImovelService service, FileStorageService fileStorageService)
        {
            _services = service;
            _fileStorageService = fileStorageService;
        }
        #endregion

        #region Controllers
        [HttpGet]
        public async Task<ActionResult<List<ImagemImovel>>> GetAll()
        {
            try
            {
                var result = await _services.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImagemImovel>> GetById(int id)
        {
            try
            {
                var result = await _services.GetById(id);
                if (result == null) return NotFound("Imagem não encontrada");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("filter/imovel/{imovelId}")]
        public async Task<ActionResult<List<ImagemImovel>>> GetByImovelId(int imovelId)
        {
            try
            {
                var result = await _services.GetByImovelId(imovelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ImagemImovel>> Post(ImagemImovel imagemImovel)
        {
            try
            {
                var result = await _services.Add(imagemImovel);
                if (result == null) return BadRequest("Erro ao adicionar imagem");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ImagemImovel>> Update(int id, ImagemImovel imagemImovel)
        {
            try
            {
                var result = await _services.Update(id, imagemImovel);
                if (result == null) return NotFound("Imagem não encontrada");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var imagem = await _services.Delete(id);
                if (imagem == null) return NotFound("Imagem não encontrada");

                await _fileStorageService.DeleteImageAsync(imagem.Url);
                return Ok("Imagem deletada com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpDelete("imovel/{imovelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteByImovelId(int imovelId)
        {
            try
            {
                var imagens = await _services.GetByImovelId(imovelId);
                if (imagens == null || !imagens.Any())
                    return NotFound("Nenhuma imagem encontrada para este imóvel");

                var result = await _services.DeleteByImovelId(imovelId);
                if (!result) return NotFound("Nenhuma imagem encontrada para este imóvel");

                foreach (var imagem in imagens)
                {
                    await _fileStorageService.DeleteImageAsync(imagem.Url);
                }

                return Ok("Imagens deletadas com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPost("{imovelId}/upload")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ImagemImovel>> UploadImage(int imovelId, [FromForm] IFormFile arquivo)
        {
            try
            {
                if (arquivo == null || arquivo.Length == 0)
                    return BadRequest("Nenhum arquivo foi enviado.");

                var relativePath = await _fileStorageService.SaveImovelImageAsync(imovelId, arquivo);
                var publicUrl = _fileStorageService.BuildPublicUrl(relativePath, Request);

                var imagemImovel = new ImagemImovel
                {
                    Imovel_Id = imovelId,
                    Url = publicUrl
                };

                var result = await _services.Add(imagemImovel);
                if (result == null) return BadRequest("Erro ao salvar imagem no banco de dados.");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }
        #endregion
    }
}

