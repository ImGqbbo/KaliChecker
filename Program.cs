using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaliChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int totalChecked = 0;
            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine(@" 
 ██ ▄█▀▄▄▄       ██▓     ██▓ ▄████▄   ██░ ██ ▓█████  ▄████▄   ██ ▄█▀▓█████  ██▀███  
  ██▄█▒▒████▄    ▓██▒    ▓██▒▒██▀ ▀█  ▓██░ ██▒▓█   ▀ ▒██▀ ▀█   ██▄█▒ ▓█   ▀ ▓██ ▒ ██▒
 ▓███▄░▒██  ▀█▄  ▒██░    ▒██▒▒▓█    ▄ ▒██▀▀██░▒███   ▒▓█    ▄ ▓███▄░ ▒███   ▓██ ░▄█ ▒
 ▓██ █▄░██▄▄▄▄██ ▒██░    ░██░▒▓▓▄ ▄██▒░▓█ ░██ ▒▓█  ▄ ▒▓▓▄ ▄██▒▓██ █▄ ▒▓█  ▄ ▒██▀▀█▄  
 ▒██▒ █▄▓█   ▓██▒░██████▒░██░▒ ▓███▀ ░░▓█▒░██▓░▒████▒▒ ▓███▀ ░▒██▒ █▄░▒████▒░██▓ ▒██▒
 ▒ ▒▒ ▓▒▒▒   ▓▒█░░ ▒░▓  ░░▓  ░ ░▒ ▒  ░ ▒ ░░▒░▒░░ ▒░ ░░ ░▒ ▒  ░▒ ▒▒ ▓▒░░ ▒░ ░░ ▒▓ ░▒▓░
 ░ ░▒ ▒░ ▒   ▒▒ ░░ ░ ▒  ░ ▒ ░  ░  ▒    ▒ ░▒░ ░ ░ ░  ░  ░  ▒   ░ ░▒ ▒░ ░ ░  ░  ░▒ ░ ▒░
 ░ ░░ ░  ░   ▒     ░ ░    ▒ ░░         ░  ░░ ░   ░   ░        ░ ░░ ░    ░     ░░   ░ 
 ░  ░        ░  ░    ░  ░ ░  ░ ░       ░  ░  ░   ░  ░░ ░      ░  ░      ░  ░   ░");
            Console.WriteLine();

            if (!File.Exists("tokens.txt"))
            {
                File.WriteAllText("tokens.txt", "");

                Console.WriteLine(" [KaliChecker] Please insert your token and restart. Press any key to exit...");
                Console.ReadKey(true);

                Environment.Exit(0);
            }

            string validTokens = string.Empty;
            string invalidTokens = string.Empty;
            string lockedTokens = string.Empty;
            List<string> totalTokens = File.ReadAllLines("tokens.txt").ToList();

            foreach (string line in totalTokens)
            {
                Thread.Sleep(50);

                Task.Run(() =>
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", line);
                    var res = client.GetAsync("https://discord.com/api/v9/users/@me/library");
                    totalChecked++;
                    switch (res.Result.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            Console.WriteLine($" [KaliChecker] ({GetCurrent(validTokens, invalidTokens, lockedTokens)}/{totalTokens.Count} - {line}) => Valid");
                            validTokens += validTokens == "" ? line : Environment.NewLine + line;
                            break;
                        case HttpStatusCode.Unauthorized:
                            Console.WriteLine($" [KaliChecker] ({GetCurrent(validTokens, invalidTokens, lockedTokens)}/{totalTokens.Count} - {line}) => Invalid");
                            invalidTokens += invalidTokens == "" ? line : Environment.NewLine + line;
                            break;
                        case HttpStatusCode.Forbidden:
                            Console.WriteLine($" [KaliChecker] ({GetCurrent(validTokens, invalidTokens, lockedTokens)}/{totalTokens.Count} - {line}) => Locked");
                            lockedTokens += lockedTokens == "" ? line : Environment.NewLine + line;
                            break;
                    }
                });
            }

            while (GetCurrent(validTokens, invalidTokens, lockedTokens) - 1 != totalTokens.Count)
            {
                Thread.Sleep(100);
            }

            File.WriteAllText("valid.txt", validTokens);
            File.WriteAllText("invalid.txt", invalidTokens);
            File.WriteAllText("locked.txt", lockedTokens);

            Thread.Sleep(1000);

            Console.WriteLine(" [KaliChecker] Done checking all tokens. Press any key to exit...");
            Console.ReadKey(true);

            Environment.Exit(0);
        }

        static int GetCurrent(string validTokens, string invalidTokens, string lockedTokens)
        {
            return Strings.Split(validTokens, Environment.NewLine).Count() + Strings.Split(invalidTokens, Environment.NewLine).Count() + Strings.Split(lockedTokens, Environment.NewLine).Count();
        }
    }
}
