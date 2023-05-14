using System.Threading.Tasks.Dataflow;
using System.IO.Ports;
using Grpc.Net.Client;

class DataReceiver
{
    SerialPort _serialPort;
    TransformBlock<byte[], Int16> _dataflowBlock;

    public DataReceiver(TransformBlock<byte[], Int16> dataflowBlock)
    {
        _serialPort = new SerialPort();
        _serialPort.BaudRate = 9600;
        _serialPort.PortName = "/dev/tty99";
        _serialPort.ReceivedBytesThreshold = 2;
        _serialPort.DataReceived += this.dataReceived;
        _dataflowBlock = dataflowBlock;
    }

    public void startReceiving()
    {
        _serialPort.Open();
    }

    private void dataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        byte[] buffer = new byte[_serialPort.ReadBufferSize];
        _serialPort.Read(buffer, 0, _serialPort.ReadBufferSize);
        _dataflowBlock.Post(buffer);
    }
}

class Formatter
{
    public static Int16 format(byte[] byteArray)
    {
        return (Int16)(BitConverter.ToInt16(byteArray));
    }
}

class GrpcClient
{
    private static readonly GrpcChannel channel;
    private static readonly Greeter.GreeterClient client;

    static GrpcClient()
    {
        channel = GrpcChannel.ForAddress("http://localhost:50051");
        client = new Greeter.GreeterClient(channel);
    }

    public static int callGrpc(Int16 value)
    {
        var reply = client.Double(new HelloRequest { Value = 22 });
        Console.WriteLine($"reply {reply.Value}");
        return reply.Value;
    }
}


internal class Program
{
    private static void Main(string[] args)
    {
        var formatBlock = new TransformBlock<byte[], Int16>(Formatter.format);
        var callGrpcBlock = new TransformBlock<Int16, int>(GrpcClient.callGrpc);
        var showValueBlock = new ActionBlock<int>((int value) => Console.WriteLine(value));

        formatBlock.LinkTo(callGrpcBlock);
        callGrpcBlock.LinkTo(showValueBlock);

        var dataReceiver = new DataReceiver(formatBlock);
        dataReceiver.startReceiving();

        Console.ReadKey();
    }
}