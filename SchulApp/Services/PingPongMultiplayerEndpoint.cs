using System.Net;
using System.Net.Sockets;

namespace SchulApp.Services
{
    public static class PingPongMultiplayerEndpoint
    {
        public const int DefaultPort = 63636;

        private static readonly object SyncRoot = new object();
        private static string currentBaseAddress = ResolveInitialBaseAddress();

        public static string CurrentBaseAddress
        {
            get
            {
                lock (SyncRoot)
                {
                    return currentBaseAddress;
                }
            }
        }

        public static string UseLocalhost()
        {
            return ConfigureHost("localhost");
        }

        public static string ConfigureHost(string hostOrAddress)
        {
            if (!TryNormalizeHost(hostOrAddress, out string normalized))
            {
                throw new ArgumentException(
                    "Bitte gib eine gültige Host-IP oder einen gültigen Hostnamen ein.",
                    nameof(hostOrAddress)
                );
            }

            lock (SyncRoot)
            {
                currentBaseAddress = normalized;
            }

            return normalized;
        }

        public static bool TryNormalizeHost(
            string? hostOrAddress,
            out string normalizedBaseAddress)
        {
            normalizedBaseAddress = string.Empty;
            string input = (hostOrAddress ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            if (!input.Contains("://", StringComparison.Ordinal))
            {
                input = "http://" + input;
            }

            if (!Uri.TryCreate(input, UriKind.Absolute, out Uri? uri) ||
                string.IsNullOrWhiteSpace(uri.Host))
            {
                return false;
            }

            string scheme = uri.Scheme.ToLowerInvariant() switch
            {
                "http" => "http",
                "https" => "https",
                "ws" => "http",
                "wss" => "https",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(scheme))
            {
                return false;
            }

            int port = uri.IsDefaultPort ? DefaultPort : uri.Port;

            if (port <= 0 || port > 65535)
            {
                return false;
            }

            UriBuilder builder = new UriBuilder
            {
                Scheme = scheme,
                Host = uri.Host,
                Port = port,
                Path = "/",
                Query = string.Empty,
                Fragment = string.Empty
            };

            normalizedBaseAddress = builder.Uri.AbsoluteUri;
            return true;
        }

        public static IReadOnlyList<string> GetLocalIPv4Addresses()
        {
            try
            {
                return Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .Where(address =>
                        address.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(address))
                    .Select(address => address.ToString())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(address => address, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (SocketException)
            {
                return Array.Empty<string>();
            }
        }

        public static string GetLanConnectionText()
        {
            IReadOnlyList<string> addresses = GetLocalIPv4Addresses();

            if (addresses.Count == 0)
            {
                return $"LAN-IP nicht erkannt | Port {DefaultPort}";
            }

            return string.Join(
                " | ",
                addresses.Select(address => $"{address}:{DefaultPort}")
            );
        }

        private static string ResolveInitialBaseAddress()
        {
            string? configured =
                Environment.GetEnvironmentVariable("SCHULAPP_MULTIPLAYER_BASE_URL") ??
                Environment.GetEnvironmentVariable("SCHULAPP_API_BASE_URL");

            if (TryNormalizeHost(configured, out string normalized))
            {
                return normalized;
            }

            return $"http://localhost:{DefaultPort}/";
        }
    }
}
