using ST10441211_PROG6212_POE.Models;
using System;
using System.Collections.Generic;

namespace ST10441211_PROG6212_POE.Views
{
    public class ConsoleView
    {
        public void Clear()
        {
            Console.Clear();
        }

        public void ShowHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {title.PadRight(59)}║");
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.ResetColor();
            Console.WriteLine();
        }

        public void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Error: {message}");
            Console.ResetColor();
        }

        public void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        public void ShowWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        public void ShowInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }

        public string GetInput(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine() ?? string.Empty;
        }

        public string GetPassword(string prompt)
        {
            Console.Write($"{prompt}: ");
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        public int ShowMenu(string title, List<string> options)
        {
            ShowHeader(title);

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {options[i]}");
            }

            Console.WriteLine();
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= options.Count)
            {
                return choice;
            }

            return -1;
        }

        public void ShowDashboard(DashboardViewModel model)
        {
            ShowHeader($"Dashboard - Welcome, {model.CurrentUserName}");

            Console.WriteLine($"Role: {model.Role}\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══ My Claims ═══");
            Console.ResetColor();

            if (model.MyClaims.Count == 0)
            {
                Console.WriteLine("  No claims submitted yet.\n");
            }
            else
            {
                ShowClaimsList(model.MyClaims);
            }

            if (model.AllClaims != null && model.Role != Role.Lecturer)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n═══ All Claims (Requires Review) ═══");
                Console.ResetColor();

                if (model.AllClaims.Count == 0)
                {
                    Console.WriteLine("  No claims to review.\n");
                }
                else
                {
                    ShowClaimsList(model.AllClaims);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public void ShowClaimsList(List<ClaimModel> claims)
        {
            foreach (var claim in claims)
            {
                Console.WriteLine($"  ID: {claim.ClaimId}");
                Console.WriteLine($"  Type: {claim.ClaimType}");
                Console.WriteLine($"  Description: {claim.Description}");
                Console.WriteLine($"  Date: {claim.ClaimDate:yyyy-MM-dd}");

                Console.Write($"  Status: ");
                switch (claim.Status.ToLower())
                {
                    case "pending":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "approved":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "rejected":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.WriteLine(claim.Status);
                Console.ResetColor();

                Console.WriteLine("  " + new string('-', 50));
            }
        }

        public void WaitForKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}