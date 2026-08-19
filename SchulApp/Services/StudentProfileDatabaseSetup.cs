using Microsoft.EntityFrameworkCore;
using SchulApp.Data;

namespace SchulApp.Services
{
    public static class StudentProfileDatabaseSetup
    {
        public static async Task SicherstellenAsync()
        {
            await using SchulAppContext db = new SchulAppContext();

            await db.Database.ExecuteSqlRawAsync(
                """
                IF COL_LENGTH(N'dbo.Benutzer', N'SchuelerId') IS NULL
                BEGIN
                    ALTER TABLE dbo.Benutzer
                    ADD SchuelerId INT NULL;
                END;
                """
            );

            await db.Database.ExecuteSqlRawAsync(
                """
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_Benutzer_Schueler'
                      AND parent_object_id = OBJECT_ID(N'dbo.Benutzer')
                )
                BEGIN
                    ALTER TABLE dbo.Benutzer
                    ADD CONSTRAINT FK_Benutzer_Schueler
                        FOREIGN KEY (SchuelerId)
                        REFERENCES dbo.Schueler(SchuelerId)
                        ON DELETE SET NULL;
                END;
                """
            );

            await db.Database.ExecuteSqlRawAsync(
                """
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'UX_Benutzer_SchuelerId'
                      AND object_id = OBJECT_ID(N'dbo.Benutzer')
                )
                BEGIN
                    CREATE UNIQUE INDEX UX_Benutzer_SchuelerId
                        ON dbo.Benutzer(SchuelerId)
                        WHERE SchuelerId IS NOT NULL;
                END;
                """
            );
        }
    }
}
