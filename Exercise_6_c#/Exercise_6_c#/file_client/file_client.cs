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
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;
		TcpClient client;
		static string file;
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
         
			// TO DO Your own code
			//file_client client = new file_client();
		}
		public string PathFile
		{
			get;
			set;
		} = "../../response";

		public void saveToFile(String s)
		{
			if (!File.Exists(PathFile)){
				File.Create(PathFile);
				File.SetAttributes(PathFile, FileAttributes.Normal);
                using (StreamWriter sw = new StreamWriter(PathFile))
                {
                    sw.Write(s);
                }
			}
			else{

                using (StreamWriter sw = new StreamWriter(PathFile,true))
                {
                    sw.Write(s);
                }
			}
		}
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

			//String fil = null;
			int byteReceived = 0;
			var listBytes = new List<byte>();
			while ((byteReceived = serverStream.Read(inStream, 0, BUFSIZE)) > 0)
			{
				listBytes.AddRange(inStream);
				//bytes= new byte[byteReceived+bytes.Len](bytes+instream); 
				//fil = Encoding.ASCII.GetString(inStream, 0, byteReceived);
			
			}
			int length = listBytes.Count;
			var bytes = new byte[length];
			for (int i = 0; i < length;i++)
			{
				bytes[i] = listBytes[i];
			}
			File.WriteAllBytes(PathFile, bytes);
			Console.WriteLine("File received");
            
            
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
			s[0] = "10.0.0.2";
			s[1] = "../../kaj.jpg";
			var file_obj = new file_client(s);
			file_obj.receiveFile();

		
            

		}
	}
}
