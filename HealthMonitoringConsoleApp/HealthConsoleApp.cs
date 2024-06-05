using EmailSending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringConsoleApp
{
    class HealthConsoleApp
    {
        public static List<string> EmailList { get; } = new List<string>();
        private static readonly string password = "admin123";

        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(HealthConsoleService));

            Console.WriteLine("Please enter the password to continue:");

            string inputPassword = Console.ReadLine();
            if (inputPassword != password)
            {
                Console.WriteLine("Invalid password.");
                return;
            }

            try
            {
                host.Open();
                Console.WriteLine("Service started.");
                while (true)
                {
                    Console.WriteLine("1. Add Email");
                    Console.WriteLine("2. Delete Email");
                    Console.WriteLine("3. View All Emails");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddEmail();
                            break;
                        case "2":
                            DeleteEmail();
                            break;
                        case "3":
                            ViewAllEmails();
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
            finally
            {
                host.Close();
            }
        }

        static void AddEmail()
        {
            Console.WriteLine("Enter the email address to add:");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email address cannot be empty.");
                return;
            }

            if (EmailList.Contains(email))
            {
                Console.WriteLine("Email already exists in the list.");
                return;
            }

            EmailList.Add(email);
            Console.WriteLine("Email added successfully.");
        }

        static void DeleteEmail()
        {
            Console.WriteLine("Enter the email address to delete:");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email address cannot be empty.");
                return;
            }

            if (!EmailList.Contains(email))
            {
                Console.WriteLine("Email does not exist in the list.");
                return;
            }

            EmailList.Remove(email);
            Console.WriteLine("Email deleted successfully.");
        }

        static void ViewAllEmails()
        {
            if (EmailList.Count == 0)
            {
                Console.WriteLine("No emails found in the list.");
                return;
            }

            Console.WriteLine("List of emails:");
            foreach (var email in EmailList)
            {
                Console.WriteLine(email);
            }
        }
    }
}