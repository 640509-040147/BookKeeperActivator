using System;
using System.IO;
using System.Threading;

namespace BookKeeperActivator
{
    public class ConsoleApp
    {
        public static void Main(string[] args)
        {
            var acto = new Activator();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Title = "Book Keeper Activator";
            const string title = @"
    ╦ ╦╦═╗╔═╗╔╦╗
    ║ ║╠╦╝║╣  ║ 
    ╚═╝╩╚═╚═╝ ╩ 
--------------------
Book Keeper 5.6.3 Activator
RCE by vikram
";
            Console.Write(title);
            Thread.Sleep(500);
            if (File.Exists(Activator.appSettingFilePath))
            {
                Console.WriteLine("Using activation file: {0}",Activator.appSettingFilePath);
                Thread.Sleep(900);
                Console.WriteLine("Writing activation data to file.");
            }
            else
            {
                Console.WriteLine("No activation file found");
                Thread.Sleep(900);
                Console.WriteLine("Creating new activation file.");

            }

            Console.WriteLine("Activating for 5 years.");
            Thread.Sleep(900);
            acto.WriteActivationFile();
            using (var progress = new ProgressBar()) {
                for (int i = 0; i <= 100; i++) {
                    progress.Report((double) i / 100);
                    Thread.Sleep(30);
                }
            }
            Console.WriteLine("Congrats! your copy is now activated for 5 more years.");

            
            Console.Write("Enter any key to exit.");
            Console.ReadKey();
        }
    }
}