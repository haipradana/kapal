using Supabase;
using dotenv.net;

namespace Kapal.Services;

public static class SupabaseService
{
    private static Client? _client;

    public static async Task<Client> GetClientAsync()
    {
        if (_client is null)
        {
            // load .env
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { ".env" }, probeForEnv: true));

            var url = Environment.GetEnvironmentVariable("SUPABASE_URL")
                      ?? throw new InvalidOperationException("SUPABASE_URL not set in env");
            var key = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY")
                      ?? throw new InvalidOperationException("SUPABASE_ANON_KEY not set in env");

            var opts = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            _client = new Client(url, key, opts);
            await _client.InitializeAsync();
        }
        return _client!;
    }
}
