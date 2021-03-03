using Dapper;
using DevEvents.API.Entidades;
using DevEvents.API.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DevEvents.API.Controllers
{
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly DevEventsDbContext _dbContext;

        public EventosController(DevEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ObterEventos()
        {
            var eventos = _dbContext.Eventos.ToList();
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public IActionResult ObterEvento(int id)
        {
            var evento = _dbContext
                .Eventos
                //Inclui os dados nas propriedades de navegação da classe Evento ("join")
                .Include(x => x.Categoria)
                .Include(x => x.Usuario)
                .Include(x => x.Inscricoes)
                .SingleOrDefault(x => x.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Evento evento)
        {
            _dbContext.Eventos.Add(evento);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Evento evento)
        {
            _dbContext.Eventos.Update(evento);

            //Define "colunas" que não devem/podem ser atualizadas
            _dbContext.Entry(evento).Property(e => e.DataCadastro).IsModified = false;
            _dbContext.Entry(evento).Property(e => e.Ativo).IsModified = false;
            _dbContext.Entry(evento).Property(e => e.IdUsuario).IsModified = false;

            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Cancelar(int id)
        {
            //Dapper
            //var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

            //using (var transactionScope = new TransactionScope())
            //{
            //    using (var sqlConnection = new SqlConnection(connectionString))
            //    {
            //        var script = "UPDATE Eventos SET Ativo = 0 WHERE Id = @id";

            //        sqlConnection.Execute(script, new { id });
            //    }

            //    transactionScope.Complete();
            //}


            //Update alternativo do EF:
            //faz a consulta do registro, altera as colunas desejadas e salva as alterações
            
            var evento = _dbContext.Eventos.SingleOrDefault(e => e.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            evento.Ativo = false;

            _dbContext.SaveChanges();
            
            return NoContent();
        }

        [HttpPost("{id}/usuarios/{idUsuario}/inscrever")]
        public IActionResult Inscrever(int id, int idUsuario, [FromBody] Inscricao inscricao)
        {
            var evento = _dbContext.Eventos.SingleOrDefault(e => e.Id == id);

            if (!evento.Ativo)
            {
                return BadRequest();
            }

            _dbContext.Inscricoes.Add(inscricao);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPost("popular")]
        public IActionResult Popular()
        {
            var usuario = new Usuario
            {
                NomeCompleto = "Otávio",
                Email = "otavio@gmail.com"
            };

            var categorias = new List<Categoria>
            {
                new Categoria{Descricao = "ASP.NET Core"},
                new Categoria {Descricao = "Xamarin"},
                new Categoria {Descricao = "Flutter"}
            };

            _dbContext.Usuarios.Add(usuario);
            _dbContext.Categorias.AddRange(categorias);

            _dbContext.SaveChanges();

            return NoContent();

        }
    }
}
