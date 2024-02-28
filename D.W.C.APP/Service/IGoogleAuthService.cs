using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Newtonsoft.Json;

namespace D.W.C.APP.Service
{
    public interface IGoogleAuthService
    {
        Task<UserCredential> AuthenticateAsync();
    }

    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly ClientSecrets _clientSecrets = new ClientSecrets
        {
            ClientId = 
            ClientSecret = 
        };

        private const string RedirectUri = "https://localhost/oauth2redirect";

        public async Task<UserCredential> AuthenticateAsync()
        {
            var scopes = new[] { "email" };
            UserCredential credential;

            try
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    _clientSecrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DWCAPP.Credentials"), true));

                // Sprawdź, czy token dostępu wymaga odświeżenia
                if (credential.Token.IsExpired(SystemClock.Default))
                {
                    await credential.RefreshTokenAsync(CancellationToken.None);
                    Console.WriteLine("Token dostępu został odświeżony.");
                }
                else
                {
                    Console.WriteLine("Token dostępu jest aktualny.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas logowania: {ex.Message}");
                credential = null;
            }

            return credential;
        }

    }




    class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        // Inne pola z odpowiedzi, jeśli są potrzebne
    }

}
