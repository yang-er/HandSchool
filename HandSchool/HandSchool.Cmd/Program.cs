using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Threading.Tasks;

namespace HandSchool.Cmd
{
    class Program
    {
        void Init()
        {
            Core.Initialize();
            Console.WriteLine("Welcome to HandSchool Command Line tool!");
            Console.WriteLine();
            foreach (var sch in Core.Schools)
                Console.WriteLine("\t" + sch.SchoolId + ":\t" + sch.SchoolName);
            Console.WriteLine();

            while (true)
            {
                Console.Write("Input your destination: ");
                var name = Console.ReadLine();
                if (Core.Schools.Find((s) => s.SchoolId == name) is ISchoolWrapper sch)
                {
                    sch.PreLoad();
                    sch.PostLoad();
                    Console.WriteLine("\n" + sch.SchoolName + " loaded!\n");
                    break;
                }
            }
        }

        async Task<bool> Login()
        {
            Console.WriteLine("Now let's begin login.");
            await Core.App.Service.PrepareLogin();
            Console.WriteLine("Tips: " + Core.App.Service.Tips);

            while (true)
            {
                Console.Write("Username: ");
                Core.App.Service.Username = Console.ReadLine();
                Console.Write("Password: ");
                Core.App.Service.Password = Console.ReadLine();

                Console.Write("Do you want to re-type? [y/N]");
                if (!Console.ReadLine().ToUpper().Contains("Y")) break;
            }

            return await Core.App.Service.Login();
        }

        void OnLoginStateChanged(object sender, LoginStateEventArgs e)
        {
            if (e.State == LoginState.Succeeded)
                Console.WriteLine("Login succeeded.");
            else if (e.State == LoginState.Processing)
                Console.WriteLine("Now busy, please check your code.");
            else
                Console.WriteLine(e.InnerError);
        }
        
        async Task WebOperate()
        {
            Console.Write("Select your method: [get/post]");
            string url, topost;
            switch (Console.ReadLine().Trim().ToUpper())
            {
                case "POST":
                    Console.Write("Post url: " + Core.App.Service.WebClient.BaseAddress);
                    url = Console.ReadLine().Trim();
                    Console.Write("Post content: ");
                    topost = Console.ReadLine().Trim();
                    Console.WriteLine(await Core.App.Service.Post(url, topost));
                    break;
                case "GET":
                    Console.Write("Get url: " + Core.App.Service.WebClient.BaseAddress);
                    url = Console.ReadLine().Trim();
                    Console.WriteLine(await Core.App.Service.Get(url));
                    break;
                default:
                    break;
            }
        }

        async Task RunSynchronously()
        {
            Core.App.Service.LoginStateChanged += OnLoginStateChanged;

            if (await Login() == false)
            {
                Console.WriteLine("Seems login failed. exit.");
            }

            Console.WriteLine(Core.App.Service.WelcomeMessage);
            Console.WriteLine(Core.App.Service.CurrentMessage);
            
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("----- HandSchool.Cmd Menu -----");
                Console.WriteLine("0. Exit menu");
                Console.WriteLine("1. Web Operate");
                Console.WriteLine();
                switch (Console.ReadLine().Trim())
                {
                    case "1":
                        await WebOperate();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Method not found");
                        break;
                }
            }
        }

        void InnerMain(string[] args)
        {
            Init();
            RunSynchronously().Wait();
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            new Program().InnerMain(args);
        }
    }
}
