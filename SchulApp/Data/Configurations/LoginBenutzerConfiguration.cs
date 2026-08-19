using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchulApp.Models;

namespace SchulApp.Data.Configurations
{
    public class LoginBenutzerConfiguration
        : IEntityTypeConfiguration<LoginBenutzer>
    {
        public void Configure(
            EntityTypeBuilder<LoginBenutzer> entity)
        {
            entity.ToTable("Benutzer");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Benutzername)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.PasswortHash)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Rolle)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Vorname)
                .HasMaxLength(100);

            entity.Property(x => x.Nachname)
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .HasMaxLength(255);

            entity.Property(x => x.Profilbild)
                .HasColumnType("varbinary(max)");

            entity.Property(x => x.ErstelltAm)
                .IsRequired();

            entity.HasIndex(x => x.Benutzername)
                .IsUnique();

            entity.HasIndex(x => x.SchuelerId)
                .IsUnique()
                .HasFilter("[SchuelerId] IS NOT NULL");

            entity.HasOne<SchuelerModel>()
                .WithOne()
                .HasForeignKey<LoginBenutzer>(x => x.SchuelerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
