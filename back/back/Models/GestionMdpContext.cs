using Microsoft.EntityFrameworkCore;

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

        public virtual DbSet<CarnetAdresse> CarnetAdresses { get; set; } = null!;
        public virtual DbSet<Compte> Comptes { get; set; } = null!;
        public virtual DbSet<Groupe> Groupes { get; set; } = null!;
        public virtual DbSet<MotDePasse> MotDePasses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarnetAdresse>(entity =>
            {
                entity.ToTable("CarnetAdresse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdCompte).HasColumnName("idCompte");

                entity.HasOne(d => d.IdCompteNavigation)
                    .WithMany(p => p.CarnetAdresses)
                    .HasForeignKey(d => d.IdCompte)
                    .HasConstraintName("FK__CarnetAdr__idCom__66603565");
            });

            modelBuilder.Entity<Compte>(entity =>
            {
                entity.ToTable("Compte");

                entity.Property(e => e.Id).HasColumnName("id");

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
                    .HasConstraintName("FK__Groupe__idCompte__693CA210");

                entity.HasMany(d => d.IdComptes)
                    .WithMany(p => p.IdGroupes)
                    .UsingEntity<Dictionary<string, object>>(
                        "CompteGroupe",
                        l => l.HasOne<Compte>().WithMany().HasForeignKey("IdCompte").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__CompteGro__idCom__6D0D32F4"),
                        r => r.HasOne<Groupe>().WithMany().HasForeignKey("IdGroupe").HasConstraintName("FK__CompteGro__idGro__6C190EBB"),
                        j =>
                        {
                            j.HasKey("IdGroupe", "IdCompte").HasName("PK__CompteGr__D94C61B5D1B04032");

                            j.ToTable("CompteGroupe");

                            j.IndexerProperty<int>("IdGroupe").HasColumnName("idGroupe");

                            j.IndexerProperty<int>("IdCompte").HasColumnName("idCompte");
                        });

                entity.HasMany(d => d.IdMdps)
                    .WithMany(p => p.IdGroupes)
                    .UsingEntity<Dictionary<string, object>>(
                        "GroupeMdp",
                        l => l.HasOne<MotDePasse>().WithMany().HasForeignKey("IdMdp").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__GroupeMdp__idMdp__70DDC3D8"),
                        r => r.HasOne<Groupe>().WithMany().HasForeignKey("IdGroupe").HasConstraintName("FK__GroupeMdp__idGro__6FE99F9F"),
                        j =>
                        {
                            j.HasKey("IdGroupe", "IdMdp").HasName("PK__GroupeMd__FE1B975D22B46BC0");

                            j.ToTable("GroupeMdp");

                            j.IndexerProperty<int>("IdGroupe").HasColumnName("idGroupe");

                            j.IndexerProperty<int>("IdMdp").HasColumnName("idMdp");
                        });
            });

            modelBuilder.Entity<MotDePasse>(entity =>
            {
                entity.ToTable("MotDePasse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdCompteCreateur).HasColumnName("idCompteCreateur");

                entity.HasOne(d => d.IdCompteCreateurNavigation)
                    .WithMany(p => p.MotDePasses)
                    .HasForeignKey(d => d.IdCompteCreateur)
                    .HasConstraintName("FK__MotDePass__idCom__6383C8BA");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
