using CorretoraJenissonLuckwuAPI.EFModel.Entities;
using CorretoraJenissonLuckwuAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorretoraJenissonLuckwuAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritoController : ControllerBase
    {
        #region Variables
        private readonly FavoritoService _services;
        #endregion

        #region Constructors
        public FavoritoController(FavoritoService service)
        {
            _services = service;
        }
        #endregion

        #region Controllers
        [HttpGet]
        public async Task<ActionResult<List<Favorito>>> GetAll()
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
        public async Task<ActionResult<Favorito>> GetById(int id)
        {
            try
            {
                var result = await _services.GetById(id);
                if (result == null) return NotFound("Favorito não encontrado");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("filter/usuario/{usuarioId}")]
        public async Task<ActionResult<List<Favorito>>> GetByUsuarioId(int usuarioId)
        {
            try
            {
                var result = await _services.GetByUsuarioId(usuarioId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("filter/imovel/{imovelId}")]
        public async Task<ActionResult<List<Favorito>>> GetByImovelId(int imovelId)
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

        [HttpGet("filter/usuario/{usuarioId}/imovel/{imovelId}")]
        public async Task<ActionResult<Favorito>> GetByUsuarioAndImovel(int usuarioId, int imovelId)
        {
            try
            {
                var result = await _services.GetByUsuarioAndImovel(usuarioId, imovelId);
                if (result == null) return NotFound("Favorito não encontrado");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Favorito>> Post(Favorito favorito)
        {
            try
            {
                var result = await _services.Add(favorito);
                if (result == null) return BadRequest("Erro ao adicionar favorito. Pode ser que já esteja favoritado.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _services.Delete(id);
                if (result == null) return NotFound("Favorito não encontrado");
                return Ok("Favorito removido com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpDelete("usuario/{usuarioId}/imovel/{imovelId}")]
        public async Task<ActionResult> DeleteByUsuarioAndImovel(int usuarioId, int imovelId)
        {
            try
            {
                var result = await _services.DeleteByUsuarioAndImovel(usuarioId, imovelId);
                if (!result) return NotFound("Favorito não encontrado");
                return Ok("Favorito removido com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }
        #endregion
    }
}
