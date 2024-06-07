using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;

        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo)
        {
            _logger = logger;
            _villaRepo = villaRepo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obtener las Villas");

            IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
            IEnumerable<VillaDto> villaDtoList = VillaToVillaDtoList(villaList);

            return Ok(villaDtoList);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con Id " + id);
                return BadRequest();
            }

            var villa = await _villaRepo.Obtener(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            var villaDto = VillaToVillaDto(villa);
            return Ok(villaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "¡La villa con ese nombre ya existe!");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa modelo = VillaCreateDtoToVilla(createDto);
            await _villaRepo.Crear(modelo);

            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _villaRepo.Obtener(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _villaRepo.Remover(villa);

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa modelo = VillaUpdateDtoToVilla(updateDto);
            await _villaRepo.Actualizar(modelo);

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked: false);

            if (villa == null) return BadRequest();

            VillaUpdateDto villaDto = VillaToVillaUpdateDto(villa);
            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = VillaUpdateDtoToVilla(villaDto);
            await _villaRepo.Actualizar(modelo);

            return NoContent();
        }

        // Métodos de mapeo manual

        private VillaDto VillaToVillaDto(Villa villa)
        {
            return new VillaDto
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };
        }

        private IEnumerable<VillaDto> VillaToVillaDtoList(IEnumerable<Villa> villas)
        {
            var villaDtoList = new List<VillaDto>();
            foreach (var villa in villas)
            {
                villaDtoList.Add(VillaToVillaDto(villa));
            }
            return villaDtoList;
        }

        private Villa VillaCreateDtoToVilla(VillaCreateDto createDto)
        {
            return new Villa
            {
                Nombre = createDto.Nombre,
                Detalle = createDto.Detalle,
                ImagenUrl = createDto.ImagenUrl,
                Ocupantes = createDto.Ocupantes,
                Tarifa = createDto.Tarifa,
                MetrosCuadrados = createDto.MetrosCuadrados,
                Amenidad = createDto.Amenidad
            };
        }

        private Villa VillaUpdateDtoToVilla(VillaUpdateDto updateDto)
        {
            return new Villa
            {
                Id = updateDto.Id,
                Nombre = updateDto.Nombre,
                Detalle = updateDto.Detalle,
                ImagenUrl = updateDto.ImagenUrl,
                Ocupantes = updateDto.Ocupantes,
                Tarifa = updateDto.Tarifa,
                MetrosCuadrados = updateDto.MetrosCuadrados,
                Amenidad = updateDto.Amenidad
            };
        }

        private VillaUpdateDto VillaToVillaUpdateDto(Villa villa)
        {
            return new VillaUpdateDto
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };
        }
    }
}