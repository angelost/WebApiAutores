﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper,IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutores")]
        [AllowAnonymous]
        public async Task<ActionResult<CollecionDeRecursos<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            var dtos = mapper.Map<List<AutorDTO>>(autores);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            dtos.ForEach(dto => GenerarEnlaces(dto, esAdmin.Succeeded));

            var resultado = new CollecionDeRecursos<AutorDTO> { Valores = dtos };
            resultado.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "self", metodo: "GET"));
            if (esAdmin.Succeeded)
            {
                resultado.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "crearAutor", metodo: "POST"));
            }           

            return resultado;
        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
        [AllowAnonymous]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor =  await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)                
                .FirstOrDefaultAsync(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            GenerarEnlaces(dto, esAdmin.Succeeded);
            return dto;
        }

        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {
            autorDTO.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }), descripcion: "self", metodo: "GET"));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }), descripcion: "autor-actualizar", metodo: "PUT"));

                autorDTO.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("borrarAutor", new { id = autorDTO.Id }), descripcion: "autor-borrar", metodo: "DELETE"));
            }            
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")] // api/autores/1
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
           

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarAutor")] // api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
