using G_ASP_NET_83_EFCore04.Data;
using G_ASP_NET_83_EFCore04.Entities;
using G_ASP_NET_83_EFCore04.Entities.Enums;
using G_ASP_NET_83_EFCore04.Entities.Enums;
using G_ASP_NET_83_EFCore04.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Mail;
using static G_ASP_NET_83_EFCore04.InputHelper;

namespace G_ASP_NET_83_EFCore04
{
    internal class Program
    {
        static AppDbContext db = null!;

        static void Main(string[] args)
        {
            db = new AppDbContext();

            try { db.Database.Migrate(); }
            catch (Exception ex)
            {
                Print($"Cannot connect to the database: {(ex.InnerException ?? ex).Message}", ConsoleColor.Red);
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    National Bank - Management");
                Console.WriteLine("========================================");
                Console.WriteLine("  1) Add a new Customer");
                Console.WriteLine("  2) Open a new Account for a Customer");
                Console.WriteLine("  3) Update Account Status (Active / Closed)");
                Console.WriteLine("  4) Remove an Account from a Customer");
                Console.WriteLine("  5) List all Customers (with accounts)");
                Console.WriteLine("  0) Exit");
                Console.WriteLine("----------------------------------------");
                Console.Write("  Enter choice: ");

                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                if (choice == "0") break;

                try
                {
                    switch (choice)
                    {
                        case "1": AddCustomer(); break;
                        case "2": OpenAccount(); break;
                        case "3": UpdateAccountStatus(); break;
                        case "4": RemoveAccount(); break;
                        case "5": ListCustomers(); break;
                        default: Print("Invalid choice. Please try again.", ConsoleColor.Red); break;
                    }
                }
                catch (Exception ex)
                {
                    db.ChangeTracker.Clear();
                    Print($"Unexpected error: {(ex.InnerException ?? ex).Message}", ConsoleColor.Red);
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey(true);
            }

            db.Dispose();   
            Console.WriteLine("Goodbye!");
        }

        // ===================== Option 1 =====================
        static void AddCustomer()
        {
            Console.WriteLine("--- Add New Customer ---");

            var fullName = ReadRequired("Full Name     ");
            var nationalId = ReadNationalId();

            if (db.Customers.Any(c => c.NationalId == nationalId))
            {
                Print("A customer with this National ID already exists.", ConsoleColor.Red);
                return;
            }

            var dob = ReadDate("Date of Birth ");
            var email = ReadEmail();
            var phone = ReadPhone();
            var address = ReadRequired("Address       ", 200);

            Console.WriteLine("Customer Type:");
            Console.WriteLine("    1) Individual");
            Console.WriteLine("    2) Business");
            var type = (CustomerType)ReadChoice(2);

            var customer = new Customer
            {
                FullName = fullName,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = type
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            Console.WriteLine();
            Print($"Customer created successfully. CustomerId = {customer.Id}", ConsoleColor.Green);
        }

        // ===================== Option 2 =====================
        static void OpenAccount()
        {
            Console.WriteLine("--- Open New Account ---");

            var accountNumber = ReadCode("Account Number");

            Console.WriteLine("Account Type:");
            Console.WriteLine("    1) Savings");
            Console.WriteLine("    2) Current");
            Console.WriteLine("    3) Business");
            var accountType = (AccountType)ReadChoice(3);

            var branchCode = ReadCode("Branch Code   ");
            var customerId = ReadInt("Customer Id   ");

            Console.WriteLine("Ownership Role:");
            Console.WriteLine("    1) Primary");
            Console.WriteLine("    2) CoHolder");
            var ownership = (OwnershipType)ReadChoice(2);

            Print($"Validating branch '{branchCode}' and customer #{customerId}...", ConsoleColor.Cyan);

            if (!db.Branches.Any(b => b.Code == branchCode))
            {
                Print($"Branch '{branchCode}' does not exist.", ConsoleColor.Red);
                return;
            }
            if (!db.Customers.Any(c => c.Id == customerId))
            {
                Print($"Customer #{customerId} does not exist.", ConsoleColor.Red);
                return;
            }

            var account = db.Accounts
                .Include(a => a.CustomerAccounts)
                .FirstOrDefault(a => a.AccountNumber == accountNumber);

            bool isNewAccount = account is null;

            if (account is null)
            {
                if (ownership != OwnershipType.Primary)
                {
                    Print("The first owner of a new account must be Primary.", ConsoleColor.Red);
                    return;
                }

                account = new Account
                {
                    AccountNumber = accountNumber,
                    AccountType = accountType,
                    BranchCode = branchCode,
                    CurrentBalance = 0,
                    OpeningDate = DateTime.Now
                };
                db.Accounts.Add(account);
            }
            else
            {
                if (account.BranchCode != branchCode || account.AccountType != accountType)
                {
                    Print($"Account '{accountNumber}' already exists with a different branch or type.", ConsoleColor.Red);
                    return;
                }
                if (account.CustomerAccounts.Any(ca => ca.CustomerId == customerId))
                {
                    Print($"Customer #{customerId} is already linked to this account.", ConsoleColor.Red);
                    return;
                }
                if (ownership == OwnershipType.Primary &&
                    account.CustomerAccounts.Any(ca => ca.OwnershipType == OwnershipType.Primary))
                {
                    Print("This account already has a Primary owner. Choose CoHolder.", ConsoleColor.Red);
                    return;
                }
            }

            account.CustomerAccounts.Add(new CustomerAccount
            {
                CustomerId = customerId,
                OwnershipType = ownership,
                OwnershipStartDate = DateTime.Now,
                AccountStatus = AccountStatus.Active
            });

            db.SaveChanges();

            if (isNewAccount)
                Print($"Account '{accountNumber}' created and linked to customer {customerId} as {ownership} owner.", ConsoleColor.Green);
            else
                Print($"Account '{accountNumber}' already existed - customer {customerId} was added as {ownership} owner.", ConsoleColor.Green);
        }

        // ===================== Option 3 =====================
        static void UpdateAccountStatus()
        {
            Console.WriteLine("--- Update Account Status ---");

            var accountNumber = ReadCode("Account Number");
            var customerId = ReadInt("Customer Id   ");

            var link = db.CustomerAccounts.Find(customerId, accountNumber);
            if (link is null)
            {
                Print("No account link found for this account number and customer.", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("New Status:");
            Console.WriteLine("    1) Active");
            Console.WriteLine("    2) Closed");
            var status = (AccountStatus)ReadChoice(2);

            if (link.AccountStatus == status)
            {
                Print($"The account is already {status}.", ConsoleColor.Cyan);
                return;
            }

            link.AccountStatus = status;
            db.SaveChanges();

            Print($"Status updated to {status}.", ConsoleColor.Green);
        }

        // ===================== Option 4 =====================
        static void RemoveAccount()
        {
            Console.WriteLine("--- Remove Account From Customer ---");

            var accountNumber = ReadCode("Account Number");
            var customerId = ReadInt("Customer Id   ");

            var link = db.CustomerAccounts.Find(customerId, accountNumber);
            if (link is null)
            {
                Print("No account link found for this account number and customer.", ConsoleColor.Red);
                return;
            }

            db.CustomerAccounts.Remove(link);
            db.SaveChanges();

            Console.WriteLine();
            Print("  Ownership link deleted.", ConsoleColor.Green);

            if (!db.CustomerAccounts.Any(ca => ca.AccountNumber == accountNumber))
            {
                var account = db.Accounts.Find(accountNumber);
                if (account is not null)
                {
                    db.Accounts.Remove(account);
                    db.SaveChanges();
                    Print($"      That was the last owner - account '{accountNumber}' was also removed.", ConsoleColor.Cyan);
                }
            }
        }

        // ===================== Option 5 =====================
        static void ListCustomers()
        {
            Console.WriteLine("--- All Customers ---");
            Console.WriteLine();

            // Eager Loading
            var customers = db.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToList();

            if (customers.Count == 0)
            {
                Console.WriteLine("  (no customers yet)");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"  #{c.Id} {c.FullName} ({c.CustomerType})");

                if (c.CustomerAccounts.Count == 0)
                {
                    Console.WriteLine("      (no accounts)");
                    continue;
                }

                foreach (var ca in c.CustomerAccounts)
                {
                    var a = ca.Account;
                    Console.WriteLine(
                        $"      {a.AccountNumber,-10} {a.AccountType,-9} Balance: {a.CurrentBalance,12:N2}   " +
                        $"{ca.OwnershipType,-9} {ca.AccountStatus,-7} @ {a.Branch.Name}");
                }
            }
        }

      
    }
}