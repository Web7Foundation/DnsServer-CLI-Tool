using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnsServerCLI;

public static class Utils
{
    // indents a json string
    public static string FormatJson(string json)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }

    public static void ShowHelpText()
    {
        Console.WriteLine("\n[ALL COMMANDS]");
        Console.WriteLine("ls - List zones | Flags: '-d' (detailed list)");
        Console.WriteLine("cd <zone name> - Enter a specific zone");
        Console.WriteLine("addzone <zone name> - Add a zone");
        Console.WriteLine("delzone <zone name> - Delete a zone");
        Console.WriteLine("clear/cls - Clear the console");
        Console.WriteLine("help - Show all commands");
        Console.WriteLine("exit - logout and exit\n");
    }
}
