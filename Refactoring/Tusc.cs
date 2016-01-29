using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        private const int TotalUsers = 4;
        private const int TotalItems = 7;
        private const int MaxErrorLogins = 5;

        private static readonly CurrentUser CurrentUser = new CurrentUser();

        public static void Start(List<User> usrs, List<Product> prods)
        {
            PrintMessage("Welcome to TUSC\n---------------");


            var errorLoginCounter = 0;

            while (errorLoginCounter < MaxErrorLogins)
            {
                if (!AuthenticateUser(usrs))
                {
                    errorLoginCounter++;
                    continue;
                }


                // Show remaining balance
                double bal = 0;
                for (int i = 0; i <= TotalUsers; i++)
                {
                    User usr = usrs[i];

                    // Check that name and password match
                    if (CurrentUser.IsSameUser(usr.Name, usr.Pwd))
                    {
                        bal = usr.Bal;

                        // Show balance 
                        PrintMessage("\nYour balance is " + usr.Bal.ToString("C"));
                    }
                }

                // Show product list
                while (true)
                {
                    // Prompt for user input
                    PrintMessage("\nWhat would you like to buy?");
                    for (var i = 0; i < TotalItems; i++)
                    {
                        Product prod = prods[i];
                        Console.WriteLine(i + 1 + ": " + prod.Name + " (" + prod.Price.ToString("C") + ")");
                    }
                    Console.WriteLine(prods.Count + 1 + ": Exit");

                    // Prompt for user input
                    PrintMessage("Enter a number:");
                    string answer = Console.ReadLine();

                    if (!IsValidAnswer(answer)) continue;

                    var num = Convert.ToInt32(answer);
                    num = num - 1;

                    // Check if user entered number that equals product count
                    if (num == TotalItems)
                    {
                        // Update balance
                        foreach (var usr in usrs)
                        {
                            // Check that name and password match
                            if (CurrentUser.IsSameUser(usr.Name, usr.Pwd))
                            {
                                usr.Bal = bal;
                            }
                        }

                        // Write out new balance
                        string json = JsonConvert.SerializeObject(usrs, Formatting.Indented);
                        File.WriteAllText(@"Data/Users.json", json);

                        // Write out new quantities
                        string json2 = JsonConvert.SerializeObject(prods, Formatting.Indented);
                        File.WriteAllText(@"Data/Products.json", json2);


                        // Prevent console from closing
                        PrintMessage("\nPress Enter key to exit\n");
                        return;
                    }
                    
                    PrintMessage("\nYou want to buy: " + prods[num].Name + "\nYour balance is " + bal.ToString("C"));

                    // Prompt for user input
                    PrintMessage("Enter amount to purchase:");
                    answer = Console.ReadLine();
                    int qty = Convert.ToInt32(answer);

                    // Check if balance - quantity * price is less than 0
                    if (bal - prods[num].Price * qty < 0)
                    {
                        //Console.Clear();
                        PrintMessage("\nYou do not have enough money to buy that.", true, ConsoleColor.Red, true);
                        continue;
                    }

                    // Check if quantity is less than quantity
                    if (prods[num].Qty <= qty)
                    {
                        //Console.Clear();
                        PrintMessage("\nSorry, " + prods[num].Name + " is out of stock", true, ConsoleColor.Red, true);
                        continue;
                    }

                    // Check if quantity is greater than zero
                    if (qty > 0)
                    {
                        // Balance = Balance - Price * Quantity
                        bal = bal - prods[num].Price * qty;

                        // Quanity = Quantity - Quantity
                        prods[num].Qty = prods[num].Qty - qty;

                        //Console.Clear();
                        PrintMessage("You bought " + qty + " " + prods[num].Name + "\nYour new balance is " + bal.ToString("C"), true, ConsoleColor.Green, true);
                    }
                    else
                    {
                        // Quantity is less than zero
                        //Console.Clear();
                        PrintMessage("\nPurchase cancelled", true, ConsoleColor.Yellow, true);
                    }
                }
            }


            if(errorLoginCounter >= MaxErrorLogins) PrintMessage("\nMaximum number of trials was reached.");

            // Prevent console from closing
            PrintMessage("\nPress Enter key to exit");
            Console.ReadLine();
        }

        private static bool IsValidAnswer(string answer)
        {
            return !string.IsNullOrEmpty(answer);
        }

        private static string ReadUserInput()
        {
            return Console.ReadLine();
        }

        private static bool IsValidUserPwd(string userName, string pwd, IList<User> usrs)
        {
            for (var i = 0; i <= TotalUsers; i++)
                if (usrs[i].Name.Equals(userName) && usrs[i].Pwd.Equals(pwd)) return true;

            return false;
        }

        private static void PrintMessage(string msg, bool isUpdateConcoleColor = false, ConsoleColor consoleColor = ConsoleColor.Black, bool isResetConsoleColor = false)
        {
            if (isUpdateConcoleColor) Console.ForegroundColor = consoleColor;
            Console.WriteLine(msg);
            if (isResetConsoleColor) Console.ResetColor();
        }

        private static bool IsValidUserName(string userName, IList<User> usrs)
        {
            if (string.IsNullOrEmpty(userName)) return true;

            for (var i = 0; i <= TotalUsers; i++)
                if (usrs[i].Name.Equals(userName)) return true;

            //user name was not found
            return false;
        }

        private static bool AuthenticateUser(IList<User> usrs)
        {
            // Prompt for user input
            PrintMessage("\nEnter Username:\n");
            string userName = ReadUserInput();

            // Validate Username
            if (!string.IsNullOrEmpty(userName) && IsValidUserName(userName, usrs))
            {
                // Prompt for user input
                PrintMessage("Enter Password:\n");
                string pwd = ReadUserInput();

                if (!string.IsNullOrEmpty(pwd) && IsValidUserPwd(userName, pwd, usrs))
                {
                    // Show welcome message
                    //Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    PrintMessage("\nLogin successful! Welcome " + userName + "!");
                    Console.ResetColor();

                    CurrentUser.Name = userName;
                    CurrentUser.Pwd = pwd;

                    return true;
                }

                // Invalid Password
                //Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                PrintMessage("\nYou entered an invalid password.");
                Console.ResetColor();
            }
            else
            {
                // Invalid User
                //Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                PrintMessage("\nYou entered an invalid user.");
                Console.ResetColor();
            }

            return false;
        }
    }
}
