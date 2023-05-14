using System.IO.Ports;
using System.Threading.Tasks;

var serialPort = new SerialPort();
serialPort.BaudRate = 9600;
serialPort.PortName = "/dev/tty98";
serialPort.Open();

Func<Task> send = async () =>
{
    Int16 vaL = 11111;
    byte[] buf = BitConverter.GetBytes(vaL);
    for (int i = 0; i < buf.Length; i++)
    {
        Console.WriteLine(buf[i]);
    }
    Console.WriteLine(BitConverter.ToString(buf));
    int count = 0;
    while (true)
    {
        await Task.Delay(1000);
        serialPort.Write(buf, 0, buf.Length);
        count++;
    }
};

await send();

serialPort.Close();

