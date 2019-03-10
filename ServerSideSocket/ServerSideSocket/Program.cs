﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
    class file_server
    {
        /// <summary>
        /// The PORT
        /// </summary>
        const int PORT = 9000;
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        const int BUFSIZE = 1000;

        const string IP = "10.0.0.2";

        static TcpListener serverSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// Opretter en socket.
        /// Venter på en connect fra en klient.
        /// Modtager filnavn
        /// Finder filstørrelsen
        /// Kalder metoden sendFile
        /// Lukker socketen og programmet
        /// </summary>
        private file_server()
        {
            IPAddress ipAddress = IPAddress.Parse(IP);
            serverSocket = new TcpListener(ipAddress, PORT);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");

            while (true)
            {
                try
				{
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine(" >> Accept connection from client");
                    string serverResponse;
                    byte[] sendBytes = new Byte[BUFSIZE];
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10024];
					int bytesRec = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom,0,bytesRec);
					Console.WriteLine($"string sent: {dataFromClient} ");
					if (!File.Exists(dataFromClient))
					{

						sendBytes = BitConverter.GetBytes(0);

						networkStream.Write(sendBytes, 0, sendBytes.Length);
						sendBytes = Encoding.ASCII.GetBytes("No File found in server");
						networkStream.Write(sendBytes, 0, sendBytes.Length);

						networkStream.Flush();

						continue;
					}
					else
					{
						long fileSize = new System.IO.FileInfo(dataFromClient).Length;
						sendFile(dataFromClient, fileSize, networkStream);

					}
					clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    clientSocket.Close();
                    serverSocket.Stop();
                    Console.WriteLine(" >> exit");
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name='fileName'>
        /// The filename.
        /// </param>
        /// <param name='fileSize'>
        /// The filesize.
        /// </param>
        /// <param name='io'>
        /// Network stream for writing to the client.
        /// </param>
        private void sendFile(String fileName, long fileSize, NetworkStream io)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] mesBuf = new byte[BUFSIZE];
				byte[] sizeBuf = new byte[BUFSIZE];
				sizeBuf = BitConverter.GetBytes(fileSize);
				io.Write(sizeBuf, 0, sizeBuf.Length);
				io.Flush();
                int bytesRead = 0;
				while ((bytesRead = fs.Read(mesBuf, 0, BUFSIZE)) > 0)
                {
                    io.Write(mesBuf, 0, bytesRead);
                }
				//byte[] eof = Encoding.ASCII.GetBytes(Convert.ToString(-1));
				//io.Write(eof, 0, eof.Length);
				io.Flush();
            }
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Server starts...");
            file_server s=new file_server();
        }
    }
}