using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Data;

namespace Server
{
    class Server_chat{
        private TcpListener server;
        private TcpClient client = new();
        private IPEndPoint ipendpoint= new(IPAddress.Any, 2000);
        private readonly List<Connection> list=new();

        Connection con;

        private struct Connection{
            public NetworkStream stream;
            public StreamWriter streamW;
            public StreamReader streamR;
            public string nick;

        }

        public Server_chat()
        {
            Initialize();
         
        }

        private void Initialize()
        {
           Console.WriteLine("Server Started");
           server = new TcpListener(ipendpoint);
           server.Start();

           while(true){

            client= server.AcceptTcpClient();
            con = new Connection
            {
                stream = client.GetStream()
            };
            con.streamR = new StreamReader(con.stream);
            con.streamW = new StreamWriter(con.stream);

            con.nick = con.streamR.ReadLine();

            list.Add(con);
            Console.WriteLine(con.nick+" está conectado.");

            Thread t = new Thread(Listen_connection);
            t.Start();
           }
        }
        void Listen_connection(){
            Connection hcon = con;

            do
            {
                try{
                    string tmp = hcon.streamR.ReadLine();
                    Console.WriteLine(hcon.nick+": "+ tmp);
                    foreach(Connection c in list){
                        try{
                            c.streamW.WriteLine(hcon.nick+": "+ tmp);
                            c.streamW.Flush();
                        }catch{

                        }
                    }
                }catch{
                    list.Remove(hcon);
                    Console.WriteLine(con.nick+": se desconectou.");
                    break;

                }
            }while(true);
        }
    }
}

