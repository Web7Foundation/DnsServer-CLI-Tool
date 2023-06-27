using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DnsServerCLI;

public static class Zone
{
    public static async Task List(HttpClient client, string token, string command)
    {
        // list zones
        // command: 'ls'
        // flags: -d
        var req = await client.GetAsync($"/api/zones/list?token={token}&pageNumber=1&zonesPerPage=100");
        var res = await req.Content.ReadAsStringAsync();

        string[] args = command.Split(' ');

        if (args.Length == 1)
        {
            // simple list
            JObject jobj = JObject.Parse(res);
            int totalZones = int.Parse(jobj["response"]["totalZones"].ToString());

            for (int i = 0; i < totalZones; i++)
            {
                var name = jobj["response"]["zones"][i]["name"].ToString();
                Console.WriteLine($"{i + 1}. {name}");
            }

            return;
        }

        if (args[1] == "-d")
        {
            // detail list
            Console.WriteLine(Utils.FormatJson(res));
            return;
        }

        Console.WriteLine("Invalid arguments: Correct usage -> 'ls' Flags: '-detail'");
    }

    public static async Task Add(HttpClient client, string token, string command)
    {
        // args 1 = 'addzone'
        // args 2 = the zone's name
        // example: 'addzone example.com'

        string[] args = command.Split(' ');
        if (args.Length != 2)
        {
            Console.WriteLine("Invalid arguments: Correct usage -> 'addzone example.com'");
            return;
        }

        // check if zone exists
        string zoneName = args[1];
        
        // create zone api call
        var req = await client.GetAsync($"/api/zones/create?token={token}&zone={zoneName}&type=Primary");
        var res = await req.Content.ReadAsStringAsync();

        bool success = JObject.Parse(res)["status"].ToString() == "ok";

        if (success) 
            Console.WriteLine($"Successfully created zone: {zoneName}\n");
        else 
            Console.WriteLine($"Failed to create {zoneName}\n ");

        Console.WriteLine($"Response: {Utils.FormatJson(res)}");
    }
        

    public static async Task Delete(HttpClient client, string token, string command)
    {
        // args 1 = 'delzone'
        // args 2 = the zone's name
        // example: 'delzone example.com'

        string[] args = command.Split(' ');
        if (args.Length != 2)
        {
            Console.WriteLine("Invalid arguments: Correct usage -> 'delzone example.com'");
            return;
        }

        // check if zone exists
        string zoneName = args[1];
        bool isValid = IsValidZone(client, token, zoneName).Result;
        if (!isValid)
        {
            Console.WriteLine($"Zone '{zoneName}' does not exists");
            return;
        }

        // create zone api call
        var req = await client.GetAsync($"/api/zones/delete?token={token}&zone={zoneName}");
        var res = await req.Content.ReadAsStringAsync();

        bool success = JObject.Parse(res)["status"].ToString() == "ok";

        if (success)
            Console.WriteLine($"Successfully deleted zone: {zoneName}\n");
        else
            Console.WriteLine($"Failed to delete {zoneName}\n ");

        Console.WriteLine($"Response: {Utils.FormatJson(res)}");
    }

    public static async Task View(HttpClient client, string token, string command)
    {
        // args 1 = 'cd'
        // args 2 = the zone's name
        // example: 'viewzone example.com'
        string[] args = command.Split(' ');
        if (args.Length != 2)
        {
            Console.WriteLine("Invalid arguments: Correct usage -> 'cd example.com'");
            return;
        }

        // check if zone exists
        bool isValid = IsValidZone(client, token, args[1]).Result;
        if (!isValid)
        {
            Console.WriteLine($"Zone '{args[1]}' does not exists");
            return;
        }

        // enter viewzone loop
        await RunViewZoneLoop(client, token, args[1]);
    }

    private static async Task RunViewZoneLoop(HttpClient client, string token, string zoneName)
    {
        bool back = false;
        while (!back)
        {
            string command = "";

            try
            {
                

                Console.Write($"DnsServer/{zoneName}> ");
                command = Console.ReadLine();

                if (command.Trim() == "back") break;
            }
            catch
            {
                Console.WriteLine("Invalid command.");
                break;
            }

            switch (command.ToLower().Trim())
            {
                case "ls":
                    await ListRecordsInZone(client, token, zoneName); 
                    break;

                case "add":
                    break;
                case "clear":
                case "cls":
                    Console.Clear();
                    break;

                case "cd..": 
                case "cd ..": 
                case "cd/": 
                case "cd /": 
                    back = true;
                    break;

                case "help":
                    Console.WriteLine($"ls - lists records");
                    Console.WriteLine($"clear/cls - clear console");
                    Console.WriteLine($"cd.. OR cd/ - go back to root");
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    Console.WriteLine("Type 'help' for a list of commands.");
                    break;
            }
        }
       

    }

    public static async Task<bool> IsValidZone(HttpClient client, string token, string zoneName)
    {
        var req = await client.GetAsync($"/api/zones/records/get?token={token}&domain={zoneName}&zone={zoneName}&listZone=true");
        var res = await req.Content.ReadAsStringAsync();

        if (JObject.Parse(res)["status"].ToString() == "error")
            return false;
        else
            return true;
    }

    public static async Task ListRecordsInZone(HttpClient client, string token, string zoneName)
    {
        // get records
        var req = await client.GetAsync($"/api/zones/records/get?token={token}&domain={zoneName}&zone={zoneName}&listZone=true");
        var res = await req.Content.ReadAsStringAsync();
        Console.WriteLine("Listing records...\n");
        Console.WriteLine(Utils.FormatJson(res));
    }
}
