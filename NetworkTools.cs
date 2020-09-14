using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System;

namespace RadioTools
{
    public static class NetworkTools
    {
        public static bool Ping(IPAddress ip)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(ip, Settings.dat.timeout);
                return reply.Status == IPStatus.Success ? true : false;
            }
            catch { return false; }
        }

        public static string GetHostName(IPAddress ip)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                Logger.Println("Found hostname for: " + ip.ToString());
                return entry.HostName;
            }
            catch (SocketException){
                Logger.Println("No hostname found for: " + ip.ToString());
            }
            return null;
        }

        public static List<ConnectionDetails> Scan()
        {
            Logger.Println("Preparing threads");
            List<ConnectionDetails> alive = new List<ConnectionDetails>();
            Thread[] threads = new Thread[51];

            Stopwatch timer = Stopwatch.StartNew();
            Logger.Println("Scanning...");
            for(int i = 0; i < threads.Length; i++)
            {
                int temp = i;
                threads[temp] = new Thread(() => Work(temp*5, (temp*5)+5));
                threads[temp].Start();
            }

            foreach(Thread t in threads)
                t.Join();

            timer.Stop();
            Logger.Print("Finished scanning in: ");
            Logger.Println(timer.Elapsed.ToString());

            Logger.Println(String.Format("Found {0} active devices", alive.Count));

            foreach(ConnectionDetails con in alive)
                Logger.Println(con.IP.ToString() + "\t" + con.alive);

            return alive;

            void Work(int start, int end)
            {
                for(int i = start ; i < end; i++)
                {
                    ConnectionDetails conn = new ConnectionDetails(IPAddress.Parse(Settings.dat.subnet+i));
                    if(conn.alive)
                        alive.Add(conn);
                }
            }
        }

        public static void SetURLs(List<ConnectionDetails> targets)
        {
            StartSetThreadedJob(targets, Settings.dat.setURLcommand, Settings.dat.URL);
        }

        public static void SetVolume(List<ConnectionDetails> targets)
        {
            StartSetThreadedJob(targets, Settings.dat.setVolumeCommand, Settings.dat.volume.ToString());
        }

        private static void StartSetThreadedJob(List<ConnectionDetails> targets, string command, string value)
        {
            Thread[] threads = new Thread[targets.Count];

            for(int i = 0; i < targets.Count; i++)
            {
                int temp = i;
                threads[temp] = new Thread(() => SendCommand(targets[temp].IP, command, value));
                threads[temp].Start();
            }
            
            foreach(Thread t in threads)
                t.Join();
        }

        private static void SendCommand(IPAddress ip, string command, string value)
        {
            try{
                TcpClient tcpclnt = new TcpClient();

                tcpclnt.Connect(ip, 1999);

                string str = "mgradio_client\n";
                Stream stm = tcpclnt.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();

                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));


                byte[] ba = asen.GetBytes(str);
                stm.Write(ba, 0, ba.Length);

                byte[] ba1 = asen.GetBytes(command + "\n");
                stm.Write(ba1, 0, ba1.Length);

                byte[] ba2 = asen.GetBytes(value + "\n");
                stm.Write(ba2, 0, ba2.Length);

                byte[] bbk = new byte[100];

                k = stm.Read(bbk, 0, 100);

                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bbk[i]));

                tcpclnt.Close();
            }
            catch{
                Logger.Println("Failed to establish connection with: " + ip.ToString());
            }
        }
    }
}