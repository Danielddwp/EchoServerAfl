using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EchoServerAfl;

namespace AfleveringsOpgave5
{
    public class Program
    {

        private static readonly List<Bog> bøger = new List<Bog>()
        {
            new Bog(10, "Daniel", "5676567656766", "Mandag"),
            new Bog(11, "Daniel", "1234567891234", "Tirsdag"),
            new Bog(12, "Carl", "4567876567876", "Onsdag"),
            new Bog(13, "Magnus", "4567656565655", "Torsdag"),
            new Bog(14, "Osman", "0000999989899", "Fredag"),
        };



        static void Main(string[] args)
        {
            
            IPAddress ip = IPAddress.Parse("192.168.24.241");
            
            TcpListener serverSocket = new TcpListener(ip, 4646);
            
            serverSocket.Start();
            Console.WriteLine("Start");

            do
            {
                Task.Run(() =>
                {
                    
                    TcpClient connectionSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Server startet");
                   
                    DoClient(connectionSocket);

                });

            } while (true);

        }

        public static void DoClient(TcpClient socket)
        {
            Stream ns = socket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            string besked = sr.ReadLine();
            string answer = "";

            
            //if ()
            while (besked != null && besked != "")
            {

                string[] messageArray = besked.Split(' ');
                string param = besked.Substring(besked.IndexOf(' ') + 1);
                string command = messageArray[0];

                switch (command)
                {
                    case "GetAll":
                        sw.WriteLine("Get all received");
                        sw.WriteLine(JsonConvert.SerializeObject(bøger));
                        break;
                    case "Get":
                        sw.WriteLine( messageArray[1]);
                        sw.WriteLine(JsonConvert.SerializeObject(bøger.Find(id => id.Isbn13 == param)));
                        break;
                    case "Save":
                        sw.WriteLine("Save received");
                        Bog saveBook = JsonConvert.DeserializeObject<Bog>(param);
                        bøger.Add(saveBook);
                        break;
                    default:
                        sw.WriteLine("Fejlsøgning");
                        break;
                }

                besked = sr.ReadLine();
            }


            ns.Close();
            socket.Close();

        }
    }
}