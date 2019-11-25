using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NotifierMac
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string SCkey = "SCU62223T4fcf61ea64acbd4b5ec59af07c2917545d88f24676bce";


        static void Main(string[] args)
        {
            MainAsnyc().Wait();

        }

        public static async Task MainAsnyc()
        {
            var values = new Dictionary<string, string>();

            Console.WriteLine("username");

            var usname = Console.ReadLine();
            Console.WriteLine("password");
            var password = Console.ReadLine();
            Console.Clear();

            values.Add("username", usname);
            values.Add("password", password);

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://ta-queue.eng.utah.edu/api/login", content);

            Console.WriteLine("Connection " + response.IsSuccessStatusCode);
            var connCount = 0;
            while (true)
            {
                if (connCount == 30000)
                {
                    connCount = 0;
                    Console.WriteLine("Connection alive: " + DateTime.Now);
                }
                var get = await client.GetAsync("https://ta-queue.eng.utah.edu/api/queue/201");
                var res = await get.Content.ReadAsStringAsync();
                var ppl = res[res.Length - 2];
                if (get.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Connection dead: " + DateTime.Now);
                    break;
                }
                if (ppl - '0' > 0)
                {
                    Console.WriteLine("Time: " + DateTime.Now + "People in queue: " + ppl);

                    await client.GetAsync("https://sc.ftqq.com/SCU62223T4fcf61ea64acbd4b5ec59af07c2917545d88f24676bce.send?text=真有人来了");
                    await Task.Delay(100000);

                }

                await Task.Delay(3000);
                connCount += 3000;
            }

        }
    }
}