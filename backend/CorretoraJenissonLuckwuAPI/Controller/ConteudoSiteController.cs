using System;
using System.Collections.Generic;
using CorretoraJenissonLuckwuAPI.Models.Entities;
using CorretoraJenissonLuckwuAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorretoraJenissonLuckwuAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudoSiteController : ControllerBase
    {
        #region Variables
        private readonly ConteudoSiteService _services;
        #endregion

        #region Constructors
        public ConteudoSiteController(ConteudoSiteService service)
        {
            _services = service;
        }
        #endregion

        #region Controllers
        [HttpGet]
        public async Task<ActionResult<List<ConteudoSite>>> GetAll([FromQuery] bool incluirInativos = false)
        {
            try
            {
                var result = await _services.GetAll(incluirInativos);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConteudoSite>> GetById(int id)
        {
            try
            {
                var result = await _services.GetById(id);
                if (result == null) return NotFound("Conteúdo não encontrado");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpGet("chave/{chave}")]
        public async Task<ActionResult<ConteudoSite>> GetByChave(string chave)
        {
            try
            {
                var result = await _services.GetByChave(chave);
                if (result == null) return NotFound("Conteúdo não encontrado");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ConteudoSite>> Create(ConteudoSite conteudoSite)
        {
            try
            {
                var existente = await _services.GetByChave(conteudoSite.Chave);
                if (existente != null)
                    return Conflict("Já existe um conteúdo cadastrado com essa chave.");

                var result = await _services.Add(conteudoSite);
                if (result == null) return BadRequest("Erro ao criar conteúdo.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ConteudoSite>> Update(int id, ConteudoSite conteudoSite)
        {
            try
            {
                var result = await _services.Update(id, conteudoSite);
                if (result == null) return NotFound("Conteúdo não encontrado");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _services.Delete(id);
                if (result == null) return NotFound("Conteúdo não encontrado");
                return Ok("Conteúdo deletado com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }
        #endregion
    }
}

