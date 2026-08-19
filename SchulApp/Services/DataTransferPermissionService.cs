using SchulApp.Models;

namespace SchulApp.Services
{
    public static class DataTransferPermissionService
    {
        public static bool DarfDatenTransferVerwenden(string rolle)
        {
            return rolle == LoginBenutzer.RolleAdmin ||
                   rolle == LoginBenutzer.RolleLehrer ||
                   rolle == LoginBenutzer.RolleSchueler;
        }

        public static bool DarfExportieren(
            string rolle,
            ExportDataSet dataSet)
        {
            if (rolle == LoginBenutzer.RolleAdmin)
            {
                return true;
            }

            if (rolle == LoginBenutzer.RolleLehrer)
            {
                return dataSet == ExportDataSet.Schueler ||
                       dataSet == ExportDataSet.Stundenplan;
            }

            if (rolle == LoginBenutzer.RolleSchueler)
            {
                return dataSet == ExportDataSet.Stundenplan;
            }

            return false;
        }

        public static bool DarfSchuelerImportieren(string rolle)
        {
            return rolle == LoginBenutzer.RolleAdmin ||
                   rolle == LoginBenutzer.RolleLehrer;
        }

        public static bool DarfStundenplanImportieren(string rolle)
        {
            return rolle == LoginBenutzer.RolleAdmin ||
                   rolle == LoginBenutzer.RolleLehrer;
        }
    }
}
