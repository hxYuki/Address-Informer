using System;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Informer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Informer Start");
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json", optional: false)
                .Build();


            var para = $"{(configurationRoot.GetSection("UseIPv6").Value == "True" ? "-6" : "")} {(configurationRoot.GetSection("Authorize").Value == "True" ? $"-H \"Authorization: {configurationRoot.GetSection("Key").Value}\"" : "")} -d \"\" {configurationRoot.GetSection("Server").Value}";
            var psi = new ProcessStartInfo("curl", para)
            { RedirectStandardOutput = true, CreateNoWindow = true };

            int interval = int.Parse(configurationRoot.GetSection("Interval").Value);
            int waitTime = 60;
            while (true)
            {
                var proc = Process.Start(psi);

                if(proc == null){
                    throw new Exception("Cannot Exceute curl with parameter:\n"+ para);
                }else{
                    var res = "";
                    using (var so = proc.StandardOutput)
                    {
                        res = so.ReadToEnd();
                        res = res.Substring(1,res.Length-2);
                        if(!IPAddress.TryParse(res, out _)){
                            waitTime = 60;
                            System.Console.WriteLine("Sending failed, retries after 60 secs.");
                            Console.WriteLine("Parameter:\n"+para);
                            Console.WriteLine("message:\n" + res);
                        }else{
                            waitTime = interval;
                            System.Console.WriteLine("Success. Next sending time 5 mins later.");
                        } 
                    }
                }

                Task.Delay(waitTime * 1000).Wait();
            }
        }
    }
}
