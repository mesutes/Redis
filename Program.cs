using ServiceStack.Redis;
using ServiceStack.Script;
using StackExchange.Redis;
using System;
using System.Net.Sockets;
using System.Windows;

namespace Redis
{
    class Program
    {

        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
        private static string ChannelMessage = "Chat odası";
        private static string userName = string.Empty;


        static void Main(string[] args)
        {


            try
            {
                var client = new TcpClient("localhost", 6379);
            }
            catch (SocketException)
            {
                Console.WriteLine("Redis sunucusu faal olmadığından uygulama başlatılamıyor.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            IRedisClient RedisClient = new RedisClient();
            userName = Guid.NewGuid().ToString();

            string Select;

            do
            {

                Console.WriteLine("Lütfen yapılacak işlemi seçiniz.\n");
                Console.WriteLine("1 - Sayaç Uygulaması");
                Console.WriteLine("2 - Chat Uygulaması");
                Console.WriteLine("3 - Çıkış");

                Select = Console.ReadLine();

                switch (Select)
                {
                    case "1":
                        string CounterSelect;

                        do
                        {
                            Console.WriteLine("-------------------------------------");
                            Console.WriteLine("Lütfen yapılacak işlemi seçiniz.\n");
                            Console.WriteLine("1 - Sayacı görüntüle");
                            Console.WriteLine("2 - Sayaç değerini bir arttır");
                            Console.WriteLine("3 - Çıkış");
                            Console.WriteLine("-------------------------------------");
                            CounterSelect = Console.ReadLine();

                            if (CounterSelect == "1")
                            {

                                var value = RedisClient.Get<string>("Counter");
                                Console.WriteLine("Sayaç Değeri : " + value);

                            }
                            else if (CounterSelect == "2")
                            {

                                var value = RedisClient.IncrementValue("Counter");

                            }
                            else if (CounterSelect == "3")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Hatalı işlem seçtiniz");
                            }

                        } while (CounterSelect != "3");

                        break;

                    case "2":

                        var pubsub = connection.GetSubscriber();
                        pubsub.Subscribe(ChannelMessage, (channel, message) => MessageAction(message));
                        pubsub.Publish(ChannelMessage, $"'{userName}' odaya katıldı.");

                        while (true)
                        {
                            pubsub.Publish(ChannelMessage, $"{userName}: {Console.ReadLine()}  " +
                              $"({DateTime.Now.Hour}:{DateTime.Now.Minute})");
                        }

                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Hatalı giriş yaptınız");
                        Console.Read();
                        break;
                }

            } while (Select != "3");


        }

        static void MessageAction(string message)
        {
            int initialCursorTop = Console.CursorTop;
            int initialCursorLeft = Console.CursorLeft;

            Console.MoveBufferArea(0, initialCursorTop, Console.WindowWidth,
                                   1, 0, initialCursorTop + 1);
            Console.CursorTop = initialCursorTop;
            Console.CursorLeft = 0;

            Console.WriteLine(message);

            Console.CursorTop = initialCursorTop + 1;
            Console.CursorLeft = initialCursorLeft;
        }
    }
}
