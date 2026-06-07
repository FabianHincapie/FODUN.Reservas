using FODUN.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FODUN.DAL.Context
{
    public class FodunContext : DbContext
    {
        public FodunContext(DbContextOptions<FodunContext> options) : base(options)
        {
        }

        // DbSets - representan las tablas
        public DbSet<TipoSede> TipoSedes { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Alojamiento> Alojamientos { get; set; }
        public DbSet<Temporada> Temporadas { get; set; }
        public DbSet<FechaTemporada> FechasTemporada { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<DetalleReserva> DetalleReservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TipoSede
            modelBuilder.Entity<TipoSede>(entity =>
            {
                entity.ToTable("TipoSede");
                entity.HasKey(e => e.TipoSedeId);
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
            });

            // Sede
            modelBuilder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(e => e.SedeId);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NombreCorto).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Ciudad).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Departamento).HasMaxLength(100).IsRequired();
                entity.HasOne(e => e.TipoSede)
                      .WithMany(t => t.Sedes)
                      .HasForeignKey(e => e.TipoSedeId);
            });

            // Alojamiento
            modelBuilder.Entity<Alojamiento>(entity =>
            {
                entity.ToTable("Alojamientos");
                entity.HasKey(e => e.AlojamientoId);
                entity.Property(e => e.Numero).HasMaxLength(20).IsRequired();
                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Alojamientos)
                      .HasForeignKey(e => e.SedeId);
            });

            // Temporada
            modelBuilder.Entity<Temporada>(entity =>
            {
                entity.ToTable("Temporadas");
                entity.HasKey(e => e.TemporadaId);
                entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            });

            // FechaTemporada
            modelBuilder.Entity<FechaTemporada>(entity =>
            {
                entity.ToTable("FechasTemporada");
                entity.HasKey(e => e.FechaTemporadaId);
                entity.HasOne(e => e.Temporada)
                      .WithMany(t => t.FechasTemporada)
                      .HasForeignKey(e => e.TemporadaId);
            });

            // Tarifa
            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.ToTable("Tarifas");
                entity.HasKey(e => e.TarifaId);
                entity.Property(e => e.ValorNoche).HasColumnType("decimal(12,2)");
                entity.Property(e => e.ValorPersonaAdicional).HasColumnType("decimal(12,2)");
                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Tarifas)
                      .HasForeignKey(e => e.SedeId);
                entity.HasOne(e => e.Temporada)
                      .WithMany(t => t.Tarifas)
                      .HasForeignKey(e => e.TemporadaId);
            });

            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.UsuarioId);
                entity.Property(e => e.NroDocumento).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.NroDocumento).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.NombreCompleto).HasMaxLength(150).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Reserva
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.ToTable("Reservas");
                entity.HasKey(e => e.ReservaId);
                entity.Property(e => e.ValorTotal).HasColumnType("decimal(12,2)");
                entity.Property(e => e.Estado).HasMaxLength(20);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Reservas)
                      .HasForeignKey(e => e.UsuarioId);
                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Reservas)
                      .HasForeignKey(e => e.SedeId);
                entity.HasOne(e => e.Temporada)
                      .WithMany(t => t.Reservas)
                      .HasForeignKey(e => e.TemporadaId);
            });

            // DetalleReserva
            modelBuilder.Entity<DetalleReserva>(entity =>
            {
                entity.ToTable("DetalleReserva");
                entity.HasKey(e => e.DetalleId);
                entity.Property(e => e.ValorNoche).HasColumnType("decimal(12,2)");
                entity.Property(e => e.SubTotal).HasColumnType("decimal(12,2)");
                entity.HasOne(e => e.Reserva)
                      .WithMany(r => r.DetalleReservas)
                      .HasForeignKey(e => e.ReservaId);
                entity.HasOne(e => e.Alojamiento)
                      .WithMany(a => a.DetalleReservas)
                      .HasForeignKey(e => e.AlojamientoId);
            });
        }
    }
}