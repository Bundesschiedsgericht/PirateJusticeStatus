using Mono.Unix;
using Mono.Unix.Native;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using PirateJusticeStatus.Infrastructure;
using PirateJusticeStatus.Model;

namespace PirateJusticeStatus
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
			Global.Mail.SendAdmin("test", "test!");

			var database = Global.Database;
			database.BeginTransaction();
            database.CreateTable<Court>();
            database.CreateTable<Judge>();
			database.CommitTransaction();

			var uri = "http://localhost:8888";
            Console.WriteLine("Starting Nancy on " + uri);

            // initialize an instance of NancyHost
            var host = new NancyHost(new Uri(uri));
            host.Start();  // start hosting

			var runner = new Runner();

			while (true)
			{
				runner.RunOnce();

				for (int i = 0; i < 600; i++)
				{
					System.Threading.Thread.Sleep(1000);
				}
			}
        }
    }
}
