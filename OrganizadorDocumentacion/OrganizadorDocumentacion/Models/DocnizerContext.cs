using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OrganizadorDocumentacion.Models;

public partial class DocnizerContext : DbContext
{
    public DocnizerContext()
    {
    }

    public DocnizerContext(DbContextOptions<DocnizerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Archivo> Archivos { get; set; }

    public virtual DbSet<Campo> Campos { get; set; }

    public virtual DbSet<Iniciativa> Iniciativas { get; set; }

    public virtual DbSet<Subcampo> Subcampos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioIniciativa> UsuarioIniciativa { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Archivo>(entity =>
        {
            entity.HasKey(e => e.ArchivoId).HasName("PK__Archivos__3D24276ABC5C22E3");

            entity.Property(e => e.ArchivoId).HasColumnName("ArchivoID");
            entity.Property(e => e.CampoId).HasColumnName("CampoID");
            entity.Property(e => e.SubcampoId).HasColumnName("SubcampoID");
            entity.Property(e => e.Nombre).HasColumnName("Nombre"); // Definir la propiedad Nombre
            entity.Property(e => e.FechaModificacion).HasColumnName("FechaModificacion"); // Definir la propiedad FechaModificacion
            entity.Property(e => e.Autor).HasColumnName("Autor"); // Definir la propiedad Autor
            entity.Property(e => e.Tamano).HasColumnName("Tamano"); // Definir la propiedad Tamano

            entity.HasOne(d => d.Campo).WithMany(p => p.Archivos)
                .HasForeignKey(d => d.CampoId)
                .HasConstraintName("FK_Archivos_Campos");

            entity.HasOne(d => d.Subcampo).WithMany(p => p.Archivos)
                .HasForeignKey(d => d.SubcampoId)
                .HasConstraintName("FK_Archivos_Subcampos");
        });


        modelBuilder.Entity<Campo>(entity =>
        {
            entity.HasKey(e => e.CampoId).HasName("PK__Campos__29825076206FF8CA");

            entity.Property(e => e.CampoId).HasColumnName("CampoID");
            entity.Property(e => e.CantidadArchivos).HasDefaultValue(0);
            entity.Property(e => e.IniciativaId).HasColumnName("IniciativaID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Iniciativa).WithMany(p => p.Campos)
                .HasForeignKey(d => d.IniciativaId)
                .HasConstraintName("FK_Campos_Iniciativas");
        });

        modelBuilder.Entity<Iniciativa>(entity =>
        {
            entity.HasKey(e => e.IniciativaId).HasName("PK__Iniciati__97CE23DDA7B38891");

            entity.Property(e => e.IniciativaId).HasColumnName("IniciativaID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });


         modelBuilder.Entity<Subcampo>(entity =>
          {
              entity.HasKey(e => e.SubcampoId).HasName("PK__Subcampo__92295ED795C893C5");

              entity.Property(e => e.SubcampoId).HasColumnName("SubcampoID");
              entity.Property(e => e.CantidadArchivos).HasDefaultValue(0);
              entity.Property(e => e.Nombre)
                  .HasMaxLength(255)
                  .IsUnicode(false);
              entity.Property(e => e.ParentCampoId).HasColumnName("ParentCampoID");
              entity.Property(e => e.ParentSubcampoId).HasColumnName("ParentSubcampoID");

              entity.HasOne(d => d.ParentCampo).WithMany(p => p.Subcampos)
                  .HasForeignKey(d => d.ParentCampoId)
                  .HasConstraintName("FK_Subcampos_Campos");

              entity.HasOne(d => d.ParentSubcampo).WithMany(p => p.InverseParentSubcampo)
                  .HasForeignKey(d => d.ParentSubcampoId)
                  .HasConstraintName("FK_Subcampos_Subcampos");
          });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798183D2C07");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UsuarioIniciativa>(entity =>
        {
            entity.HasKey(e => e.UsuarioIniciativaId).HasName("PK__UsuarioI__BBA36CF4888E9D8E");

            entity.ToTable("UsuarioIniciativa");

            entity.Property(e => e.UsuarioIniciativaId).HasColumnName("UsuarioIniciativaID");
            entity.Property(e => e.IniciativaId).HasColumnName("IniciativaID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Iniciativa).WithMany(p => p.UsuarioIniciativas)
                .HasForeignKey(d => d.IniciativaId)
                .HasConstraintName("FK_UsuarioIniciativa_Iniciativas");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioIniciativas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_UsuarioIniciativa_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
