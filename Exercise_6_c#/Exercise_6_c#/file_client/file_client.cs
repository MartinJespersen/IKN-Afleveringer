using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
namespace tcp
{
	class file_client
	{
		/// <summary>
		/// The PORT.
		/// </summary>
		private const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;      
		private TcpClient client;
		private string file;
		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		private file_client (string[] args)
		{
			file = args[1];
            Console.WriteLine("Client starts...");
            client = new TcpClient();
            client.Connect(args[0], PORT);         
            receiveFile();       
		}
		public string PathFile{ get; set; } = "../../response";

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		private void receiveFile ()
		{
			NetworkStream serverStream = client.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes(file);
            serverStream.Write(outStream, 0, outStream.Length);
			serverStream.Flush();
			byte[] inStream= new byte[BUFSIZE];
			int byteReceived = 0;
			if (File.Exists(PathFile))
				File.Delete(PathFile);
			//int count = 0;
			//int sizeOfMes = 0;
			byteReceived = serverStream.Read(inStream, 0, BUFSIZE);
			int fileSize = BitConverter.ToInt32(inStream,0);
			if (fileSize == 0)
			{
				byteReceived = serverStream.Read(inStream, 0, BUFSIZE);
				string error = System.Text.Encoding.ASCII.GetString(inStream, 0, byteReceived);
				Console.WriteLine(error);
			}
			else
			{
				while ((byteReceived = serverStream.Read(inStream, 0, BUFSIZE)) > 0)
				{
					//if (byteReceived != BUFSIZE)
						//sizeOfMes = byteReceived;
					WriteToFile(inStream);
					//count++;
				}
				//sizeOfMes += count * BUFSIZE;
                Console.WriteLine($"File received with {fileSize} bytes");
			}
		}

        private void WriteToFile(byte[] bytes)
		{
            try
            {
				using (var fs = new FileStream(PathFile,FileMode.Append))
                {
					fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
            }
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			var s = new string[2];
			//s[0] = "10.0.0.2";
			//s[1] = "../../kaj.jpg";
			var file_obj = new file_client(args);  
		}
	}
}
