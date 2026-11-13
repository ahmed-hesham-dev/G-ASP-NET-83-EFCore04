using System.Globalization;
using System.Net.Mail;

namespace G_ASP_NET_83_EFCore04
{
    public static class InputHelper
    {
        public static void Print(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static string ReadRequired(string label, int maxLength = 100)
        {
            while (true)
            {
                Console.Write($"{label} : ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    Print("This field is required. Try again.", ConsoleColor.Red);
                else if (input.Length > maxLength)
                    Print($"Maximum length is {maxLength} characters.", ConsoleColor.Red);
                else
                    return input;
            }
        }

        public static string ReadNationalId()
        {
            while (true)
            {
                Console.Write("National ID    : ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input) || input.Length > 14 || !input.All(char.IsDigit))
                    Print("National ID must contain digits only (max 14).", ConsoleColor.Red);
                else
                    return input;
            }
        }

        public static DateTime ReadDate(string label)
        {
            while (true)
            {
                Console.Write($"{label} : (yyyy-MM-dd) ");
                var input = Console.ReadLine()?.Trim();

                if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var date))
                {
                    if (date > DateTime.Today)
                        Print("Date cannot be in the future.", ConsoleColor.Red);
                    else if (date < new DateTime(1900, 1, 1))
                        Print("Date is too old.", ConsoleColor.Red);
                    else
                        return date;
                }
                else
                    Print("Invalid date. Use the format yyyy-MM-dd.", ConsoleColor.Red);
            }
        }

        public static string ReadEmail()
        {
            while (true)
            {
                Console.Write("Email          : ");
                var input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(input) && input.Length <= 100 &&
                    MailAddress.TryCreate(input, out var addr) &&
                    string.Equals(addr.Address, input, StringComparison.OrdinalIgnoreCase))
                    return input;

                Print("Invalid email address.", ConsoleColor.Red);
            }
        }

        public static string ReadPhone()
        {
            while (true)
            {
                Console.Write("Phone          : ");
                var input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(input) && input.Length is >= 7 and <= 15 && input.All(char.IsDigit))
                    return input;

                Print("Phone must be 7-15 digits.", ConsoleColor.Red);
            }
        }

        public static int ReadChoice(int max)
        {
            while (true)
            {
                Console.Write("  Choice: ");
                if (int.TryParse(Console.ReadLine(), out var n) && n >= 1 && n <= max)
                    return n;

                Print($"Please enter a number between 1 and {max}.", ConsoleColor.Red);
            }
        }

        public static int ReadInt(string label)
        {
            while (true)
            {
                Console.Write($"{label} : ");
                if (int.TryParse(Console.ReadLine(), out var n) && n > 0)
                    return n;

                Print("Please enter a valid positive number.", ConsoleColor.Red);
            }
        }

        public static string ReadCode(string label, int maxLength = 20)
        {
            while (true)
            {
                Console.Write($"{label} : ");
                var input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(input) && input.Length <= maxLength && !input.Contains(' '))
                    return input.ToUpper();

                Print($"Required, no spaces, max {maxLength} characters.", ConsoleColor.Red);
            }
        }
    }
}