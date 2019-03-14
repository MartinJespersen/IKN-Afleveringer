using System;
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
        /// <summary>
        /// The IP of the Server
        /// </summary>
        const string IP = "10.0.0.2";

        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// Create a socket.
        /// Wait for client to connect.
        /// Receive filename
        /// Find filesize
        /// Call send file
        /// Close socket
        /// </summary>
        private file_server()
        {
            IPAddress ipAddress = IPAddress.Parse(IP);
            var serverSocket = new TcpListener(ipAddress, PORT);
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
                    byte[] sendBytes = new Byte[BUFSIZE];
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10024];
                    int bytesRec = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom, 0, bytesRec);
                    Console.WriteLine($"string sent: {dataFromClient} ");               
					long fileSize = File.Exists(dataFromClient) ? new System.IO.FileInfo(dataFromClient).Length : 0;
                    sendBytes = BitConverter.GetBytes(fileSize);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
					if (fileSize == 0)
                    {

                        sendBytes = Encoding.ASCII.GetBytes("No File found in server");
                        networkStream.Write(sendBytes, 0, sendBytes.Length);

                        networkStream.Flush();
                    }
					else
					{                  
                        sendFile(dataFromClient, fileSize, networkStream);
					}               
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    serverSocket.Stop();
                    Console.WriteLine(" >> exit");
                    Console.ReadLine();
                }
				finally
				{
					clientSocket.Close();
				}
            }
        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name='fileName'> The filename.</param>
        /// <param name='fileSize'> The filesize.</param>
        /// <param name='io'> Network stream for writing to the client.</param>
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
                io.Flush();
            }
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>The command-line arguments</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Server starts...");
            file_server s = new file_server();
        }
    }
}