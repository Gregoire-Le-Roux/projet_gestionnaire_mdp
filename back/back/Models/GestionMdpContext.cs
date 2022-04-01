using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace back.Models
{
    public partial class GestionMdpContext : DbContext
    {
        public GestionMdpContext()
        {
        }

        public GestionMdpContext(DbContextOptions<GestionMdpContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Compte> Comptes { get; set; } = null!;
        public virtual DbSet<Groupe> Groupes { get; set; } = null!;
        public virtual DbSet<MotDePasse> MotDePasses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compte>(entity =>
            {
                entity.ToTable("Compte");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.HashCle)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("hashCle");

                entity.Property(e => e.Mail)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("mail");

                entity.Property(e => e.Mdp)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("mdp");

                entity.Property(e => e.Nom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("nom");

                entity.Property(e => e.Prenom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("prenom");

                entity.HasMany(d => d.IdCompteCarnets)
                    .WithMany(p => p.IdComptes)
                    .UsingEntity<Dictionary<string, object>>(
                        "CarnetAdresse",
                        l => l.HasOne<Compte>().WithMany().HasForeignKey("IdCompteCarnet").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__CarnetAdr__idCom__1BC821DD"),
                        r => r.HasOne<Compte>().WithMany().HasForeignKey("IdCompte").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__CarnetAdr__idCom__1AD3FDA4"),
                        j =>
                        {
                            j.HasKey("IdCompte", "IdCompteCarnet").HasName("PK__CarnetAd__E66F3CC5B389B155");

                            j.ToTable("CarnetAdresse");

                            j.IndexerProperty<int>("IdCompte").HasColumnName("idCompte");

                            j.IndexerProperty<int>("IdCompteCarnet").HasColumnName("idCompteCarnet");
                        });

                entity.HasMany(d => d.IdComptes)
                    .WithMany(p => p.IdCompteCarnets)
                    .UsingEntity<Dictionary<string, object>>(
                        "CarnetAdresse",
                        l => l.HasOne<Compte>().WithMany().HasForeignKey("IdCompte").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__CarnetAdr__idCom__1AD3FDA4"),
                        r => r.HasOne<Compte>().WithMany().HasForeignKey("IdCompteCarnet").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__CarnetAdr__idCom__1BC821DD"),
                        j =>
                        {
                            j.HasKey("IdCompte", "IdCompteCarnet").HasName("PK__CarnetAd__E66F3CC5B389B155");

                            j.ToTable("CarnetAdresse");

                            j.IndexerProperty<int>("IdCompte").HasColumnName("idCompte");

                            j.IndexerProperty<int>("IdCompteCarnet").HasColumnName("idCompteCarnet");
                        });
            });

            modelBuilder.Entity<Groupe>(entity =>
            {
                entity.ToTable("Groupe");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdCompteCreateur).HasColumnName("idCompteCreateur");

                entity.Property(e => e.Titre)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("titre");

                entity.HasOne(d => d.IdCompteCreateurNavigation)
                    .WithMany(p => p.Groupes)
                    .HasForeignKey(d => d.IdCompteCreateur)
                    .HasConstraintName("FK__Groupe__idCompte__1EA48E88");

                entity.HasMany(d => d.IdComptes)
                    .WithMany(p => p.IdGroupes)
                    .UsingEntity<Dictionary<string, object>>(
                        "CompteGroupe",
                        l => l.HasOne<Compte>().WithMany().HasForeignKey("IdCompte").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__CompteGro__idCom__22751F6C"),
                        r => r.HasOne<Groupe>().WithMany().HasForeignKey("IdGroupe").HasConstraintName("FK__CompteGro__idGro__2180FB33"),
                        j =>
                        {
                            j.HasKey("IdGroupe", "IdCompte").HasName("PK__CompteGr__D94C61B5D076C8D5");

                            j.ToTable("CompteGroupe");

                            j.IndexerProperty<int>("IdGroupe").HasColumnName("idGroupe");

                            j.IndexerProperty<int>("IdCompte").HasColumnName("idCompte");
                        });

                entity.HasMany(d => d.IdMdps)
                    .WithMany(p => p.IdGroupes)
                    .UsingEntity<Dictionary<string, object>>(
                        "GroupeMdp",
                        l => l.HasOne<MotDePasse>().WithMany().HasForeignKey("IdMdp").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__GroupeMdp__idMdp__2645B050"),
                        r => r.HasOne<Groupe>().WithMany().HasForeignKey("IdGroupe").HasConstraintName("FK__GroupeMdp__idGro__25518C17"),
                        j =>
                        {
                            j.HasKey("IdGroupe", "IdMdp").HasName("PK__GroupeMd__FE1B975DB7BC5E73");

                            j.ToTable("GroupeMdp");

                            j.IndexerProperty<int>("IdGroupe").HasColumnName("idGroupe");

                            j.IndexerProperty<int>("IdMdp").HasColumnName("idMdp");
                        });
            });

            modelBuilder.Entity<MotDePasse>(entity =>
            {
                entity.ToTable("MotDePasse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateExpiration)
                    .HasColumnType("date")
                    .HasColumnName("dateExpiration");

                entity.Property(e => e.Description)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.IdCompteCreateur).HasColumnName("idCompteCreateur");

                entity.Property(e => e.Login)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("login");

                entity.Property(e => e.Mdp)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("mdp");

                entity.Property(e => e.Titre)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("titre");

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("url");

                entity.HasOne(d => d.IdCompteCreateurNavigation)
                    .WithMany(p => p.MotDePasses)
                    .HasForeignKey(d => d.IdCompteCreateur)
                    .HasConstraintName("FK__MotDePass__idCom__17F790F9");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
