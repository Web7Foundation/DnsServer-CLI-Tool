using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DnsServerCLI;

class Program
{
    const string ADDRESS = "http://localhost:5380";

    private static readonly HttpClient client = new HttpClient
    {
        BaseAddress = new Uri(ADDRESS)
    };

    static async Task Main(string[] args)
    {
        // set args for testing:
        // DnsServerCLI Debug Properties > Command line arguments > set you username and password
        if (args.Length != 2)
        {
            Console.WriteLine("Invalid arguments. Usage: dnsserver <username> <password>");
            return;
        }

        // get login token
        var token = await Login(args[0], args[1]);

        bool running = true;      
        while (running)
        {
            string command = "";

            try
            {
                Console.Write("DnsServer> ");
                command = Console.ReadLine() ?? "";
            }
            catch { }
            
            switch (command.ToLower().Trim())
            {
                #region zones

                case string c when c.StartsWith("ls"):
                    await Zone.List(client, token, command);
                    break;

                case string c when c.StartsWith("cd"):
                    await Zone.View(client, token, command);
                    break;

                case string c when c.StartsWith("addzone"):
                    await Zone.Add(client, token, command);
                    break;

                case string c when c.StartsWith("delzone"):
                    await Zone.Delete(client, token, command);
                    break;

                #endregion

                case "cls":
                case "clear":
                    Console.Clear();
                    break;

                case "help":
                    Utils.ShowHelpText();
                    break;

                case "exit":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid command.");
                    Console.WriteLine("Type 'help' for a list of commands.");
                    break;
            }

        }

        bool success = await Logout(token);

        if (success) 
            Console.WriteLine("Logged out successfully.");
        else 
            Console.WriteLine("Failed to logout.");
    }       

    private static async Task<bool> Logout(string token)
    {
        // logout with token
        var req = await client.PostAsync($"/api/user/logout?token={token}", null);
        var res = await req.Content.ReadAsStringAsync();

        return JObject.Parse(res)["status"].ToString() == "ok";
    }

    private static async Task<string> Login(string username, string password)
    {
        // get login token
        try
        {
            var req = await client.GetAsync($"/api/user/login?user={username}&pass={password}&includeInfo=true");
            string res = await req.Content.ReadAsStringAsync();
            JObject.Parse(res).TryGetValue("token", out JToken jtoken);

            if (jtoken != null) 
            {
                Console.WriteLine($"Successfully logged in to {ADDRESS}");
                return jtoken.ToString();
            } 
            else
            {
                Console.WriteLine($"[FAILED] Invalid username/password");
                Environment.Exit(-1);
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"[ERROR] {ex.Message}");
            Environment.Exit(-1);
        }

        throw new Exception("Failed to login to Dns Server.");
    }  

}