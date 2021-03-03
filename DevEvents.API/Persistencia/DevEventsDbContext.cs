using DevEvents.API.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEvents.API.Persistencia
{
    public class DevEventsDbContext : DbContext
    {
        public DevEventsDbContext(DbContextOptions<DevEventsDbContext> options) : base(options)
        {

        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }


        //Configuração de relacionamentos e caracteristicas/propriedades
        //das tabelas e colunas. (Fluent API)
        //Também é possivel fazer por dataAnnotations, porém desta
        //forma as entidades/dominios ficam mais limpos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>()
                .HasKey(x => x.Id);//pk

            modelBuilder.Entity<Evento>()
                .HasOne(x => x.Categoria)//Um evento tem uma categoria
                .WithMany()//Uma categoria tem muitos eventos
                .HasForeignKey(x => x.IdCategoria);//IdCategoria é fk

            modelBuilder.Entity<Evento>()
                .HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.IdUsuario);

            modelBuilder.Entity<Evento>()
                .Property(x => x.Descricao)
                .HasMaxLength(100);

            modelBuilder.Entity<Categoria>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Usuario>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Inscricao>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Inscricao>()
                .HasOne(x => x.Evento)
                .WithMany(e => e.Inscricoes)
                .HasForeignKey(x => x.IdEvento)
                .OnDelete(DeleteBehavior.Restrict);//Não permite exclusão de evento caso haja inscrições vinculadas

            modelBuilder.Entity<Inscricao>()
                .HasOne(x => x.Usuario)
                .WithMany(e => e.Inscricoes)
                .HasForeignKey(x => x.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);//Não permite exclusão de evento caso haja usuarios vinculadas
        }

    }
}
