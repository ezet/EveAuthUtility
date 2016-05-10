using System;
using eZet.EveLib.EveAuthModule;

namespace ezet.EveAuthUtility {
    public class EveAuthUtility {

        public static EveAuth Auth = new EveAuth();

        [STAThread]
        public static void Main(string[] args) {
            Console.WriteLine("Simple tool for obtaining access and refresh tokens from the Eve Online SSO.");
            Console.WriteLine("No information is stored, some data is exchanged with the eve login servers.");
            Console.WriteLine("The sourcecode is available here: https://github.com/ezet/evelib");
            Console.WriteLine("You need to register an application at https://developers.eveonline.com, \nthe callback URL can be set to '/'.");
            Console.WriteLine("Make sure you enable CREST and add the publicData scope for your application!\n");
            Console.WriteLine("Your client ID and secret key will be provided by \nhttps://developers.eveonline.com after registering an application.\n");

            Console.WriteLine("For easier editing, rightclick the title bar for the console window, \nthen go to Properties -> Options -> enable QuickEdit mode.\n");
            Console.Write("Please select SSO server: (1) Tranquility or (2) Singularity: ");
            var server = Console.ReadLine();
            if (server == "2") Auth.Host = "sisilogin.testeveonline.com";
            Console.WriteLine("Enter your client ID: ");
            var clientId = "46daa2b378bd4bc189df4c3a73af226a"; // TRANQ
            clientId = Console.ReadLine();
            // var clientId = "cefe601d9f5a444183f8c732676709fb"; // SISI
            Console.WriteLine("Enter your secret key: ");
            var secret = Console.ReadLine();
            //secret = "Gwg3JNT8V0DLZwb7ZmRke9zJDYp1ePnUm9V5zvjY"; // SISI
            //secret = "K8GcWADljgnLZyrKGFfiqzHVvViGhapOYSCEy83h";  // TRANQ
            var encodedKey = EveAuth.Encode(clientId, secret);
            Console.WriteLine("Please enter your request scopes as a space delimited string, or leave empty for default scopes: ");
            var scopes = "publicData characterFittingsRead characterFittingsWrite characterContactsRead characterContactsWrite characterLocationRead characterNavigationWrite";
            var newScopes = Console.ReadLine();
            if (!string.IsNullOrEmpty(newScopes)) scopes = newScopes;
            var authLink = Auth.CreateAuthLink(clientId, "/", "default", scopes);
            System.Windows.Forms.Clipboard.SetText(authLink);
            Console.WriteLine("Please log in using the following link (already copied to your clipboard): ");
            Console.WriteLine(authLink);
            Console.WriteLine("After logging in, copy the full URL from your browser.");
            Console.WriteLine("Enter the full URL: ");
            string url = Console.ReadLine();
            string authCode = "";
            try {
                int start = url.IndexOf("?code=", System.StringComparison.Ordinal);
                int end = url.IndexOf("&state", System.StringComparison.Ordinal);
                authCode = url.Substring(start + 6, end - start - 6);
            }
            catch (Exception) {
                Console.WriteLine("Unable to locate authentication code, please try again.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Authentication code found: " + authCode);
            Console.WriteLine("Authenticating...");
            AuthResponse response;
            try {
                response = Auth.AuthenticateAsync(encodedKey, authCode).Result;
            }
            catch (Exception e) {
                Console.WriteLine("Authentication unsuccessfull, please try again.");
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Authentication successfull!");
            Console.WriteLine("\nAccess token:\n" + response.AccessToken);
            Console.WriteLine("\nRefresh token:\n" + response.RefreshToken);
            Console.WriteLine("\nEncoded key:\n" + encodedKey);
            Console.ReadLine();
        }
    }
}
