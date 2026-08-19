using SchulApp.Models;
using SchulApp.Services;
using Xunit;

namespace SchulApp.Tests
{
    public class DataTransferPermissionServiceTests
    {
        [Fact]
        public void Lehrer_DarfSchuelerUndStundenplanExportieren()
        {
            Assert.True(
                DataTransferPermissionService.DarfExportieren(
                    LoginBenutzer.RolleLehrer,
                    ExportDataSet.Schueler
                )
            );

            Assert.True(
                DataTransferPermissionService.DarfExportieren(
                    LoginBenutzer.RolleLehrer,
                    ExportDataSet.Stundenplan
                )
            );

            Assert.False(
                DataTransferPermissionService.DarfExportieren(
                    LoginBenutzer.RolleLehrer,
                    ExportDataSet.AuditLog
                )
            );
        }

        [Fact]
        public void Schueler_DarfNurStundenplanExportieren()
        {
            Assert.True(
                DataTransferPermissionService.DarfExportieren(
                    LoginBenutzer.RolleSchueler,
                    ExportDataSet.Stundenplan
                )
            );

            Assert.False(
                DataTransferPermissionService.DarfExportieren(
                    LoginBenutzer.RolleSchueler,
                    ExportDataSet.Schueler
                )
            );

            Assert.False(
                DataTransferPermissionService.DarfExportieren(
                    LoginBenutzer.RolleSchueler,
                    ExportDataSet.Klassen
                )
            );
        }

        [Fact]
        public void Import_IstNurFuerAdminUndLehrerErlaubt()
        {
            Assert.True(
                DataTransferPermissionService.DarfSchuelerImportieren(
                    LoginBenutzer.RolleAdmin
                )
            );
            Assert.True(
                DataTransferPermissionService.DarfSchuelerImportieren(
                    LoginBenutzer.RolleLehrer
                )
            );
            Assert.False(
                DataTransferPermissionService.DarfSchuelerImportieren(
                    LoginBenutzer.RolleSchueler
                )
            );

            Assert.True(
                DataTransferPermissionService.DarfStundenplanImportieren(
                    LoginBenutzer.RolleAdmin
                )
            );
            Assert.True(
                DataTransferPermissionService.DarfStundenplanImportieren(
                    LoginBenutzer.RolleLehrer
                )
            );
            Assert.False(
                DataTransferPermissionService.DarfStundenplanImportieren(
                    LoginBenutzer.RolleSchueler
                )
            );
        }

        [Fact]
        public void Admin_BehaeltVollzugriffAufExporte()
        {
            foreach (ExportDataSet dataSet in Enum.GetValues<ExportDataSet>())
            {
                Assert.True(
                    DataTransferPermissionService.DarfExportieren(
                        LoginBenutzer.RolleAdmin,
                        dataSet
                    )
                );
            }
        }
    }
}
