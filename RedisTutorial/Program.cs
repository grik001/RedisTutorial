using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://github.com/MicrosoftArchive/redis/releases
            //https://redisdesktop.com/

            string read = "";

            Console.WriteLine("Starting bulk Save to Cache!");
            var database = GetRedisConnection();

            List<Guid> keyStore = new List<Guid>();

            //Insert Data
            for (int i = 0; i < 50000; i++)
            {
                var key = Guid.NewGuid();
                keyStore.Add(key);
                database.StringSet(key.ToString(), $"{i}:{key}");
            }

            Console.WriteLine("Bulk Data inserted: Press enter to load data generated?");
            read = Console.ReadLine();

            //Read Data by Keys
            foreach (var key in keyStore)
            {
                var cacheValue = database.StringGet(key.ToString());
                Console.WriteLine(cacheValue.ToString());
            }

            List<Guid> modifiedKeys = new List<Guid>();

            //Read and Append Data
            foreach (var key in keyStore.Skip(new Random().Next(0, 50000)).Take(100))
            {
                var cacheValue = database.StringAppend(key.ToString(), $":{Guid.NewGuid().ToString()}");
                modifiedKeys.Add(key);
            }

            Console.WriteLine("Press enter to load modified generated?");
            read = Console.ReadLine();

            //Read Modified Keys by Keys
            foreach (var key in modifiedKeys)
            {
                var cacheValue = database.StringGet(key.ToString());
                Console.WriteLine(cacheValue.ToString());
            }

            Console.ReadLine();  
        }

        public static IDatabase GetRedisConnection()
        {
            var serverName = "localhost:6379";
            var connection = ConnectionMultiplexer.Connect($"{serverName},allowAdmin=true");

            var server = connection.GetServer(serverName);
            server.FlushDatabase(2);

            var database = connection.GetDatabase(2);
            return database;
        }
    }
}
