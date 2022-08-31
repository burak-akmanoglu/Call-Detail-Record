using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

String data;
TcpListener server = null;
try
{
    // Set the TcpListener on port 13000.
    Int32 port = 13000;
    IPAddress localAddr = IPAddress.Parse("192.168.1.38");

    // TcpListener server = new TcpListener(port);
    server = new TcpListener(localAddr, port);

    // Start listening for client requests.
    server.Start();

    // Buffer for reading data
    Byte[] bytes = new Byte[256];

    // Enter the listening loop.
    while (true)
    {
        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected!");

        data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        int i;

        // Loop to receive all the data sent by the client.
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("Received: {0}", data);

            // Process the data sent by the client.
            data = data.ToUpper();

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

            // Send back a response.
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", data);

            db();
            Txt();
        }

        // Shutdown and end connection
        client.Close();
    }
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
finally
{
    // Stop listening for new clients.
    server.Stop();
}

Console.WriteLine("\nHit enter to continue...");
Console.Read();

void db()
{
    string[] words = data.Split(',');

    //foreach (var word in words)
    //{
    //    Console.WriteLine(word);

    //}
    SqlConnection connec = new SqlConnection("Data Source=BURAK;Initial Catalog=Innova;Integrated Security=True");
    connec.Open();
    if (connec.State == ConnectionState.Open)
    {
        Console.WriteLine("ok");
    }
    if (words.Length == 7)
    {
        SqlCommand cmd = new SqlCommand("insert into TblClientData(TelephoneNumber,TargetTelephoneNumber,Date,RingTime,CallTime,StartTime,FinishTime) values('" + words[0] + "','" + words[1] + "','" + words[2] + "','" + words[3] + "','" + words[4] + "','" + words[5] + "','" + words[6] + "')", connec);
        int sonuc = cmd.ExecuteNonQuery();
        Console.WriteLine("Veri tabanına eklendi" + sonuc);
    }
    else
    {
        Console.WriteLine("Veri tabanına ekleme yapılamadı");
    }
}
void Txt()
{
    string path = $@"C:\Users\Burak\Desktop\innova\{data}.txt";

    try
    {
        // Create the file, or overwrite if the file exists.
        using (FileStream fs = File.Create(path))
        {
            byte[] info = new UTF8Encoding(true).GetBytes($"Client Says:{data}");
            // Add some information to the file.
            fs.Write(info, 0, info.Length);
        }

        // Open the stream and read it back.
        using (StreamReader sr = File.OpenText(path))
        {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                //  Console.WriteLine(s);
            }
        }
    }

    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}