namespace SchulApp.Services
{
    internal static class ApiAuditHeaders
    {
        public static void Anwenden(
            HttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(
                    BenutzerSession.Benutzername))
            {
                request.Headers.TryAddWithoutValidation(
                    "X-SchulApp-User",
                    BenutzerSession.Benutzername
                );
            }

            if (!string.IsNullOrWhiteSpace(
                    BenutzerSession.Rolle))
            {
                request.Headers.TryAddWithoutValidation(
                    "X-SchulApp-Role",
                    BenutzerSession.Rolle
                );
            }
        }
    }
}
