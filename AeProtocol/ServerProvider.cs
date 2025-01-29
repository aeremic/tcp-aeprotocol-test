using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AeProtocol;

public class ServerProvider
{
    private readonly IPEndPoint? _ipEndPoint;

    private Socket? _server;
    private Socket? _handler;

    public event EventHandler<string>? MessageReceivedCallback;

    public ServerProvider(int port)
    {
        var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostEntry.AddressList[0];

        _ipEndPoint = new IPEndPoint(ipAddress, port);

        _server = null;
        _handler = null;
    }

    public async Task ConnectAsync()
    {
        if (_ipEndPoint == null)
        {
            throw new NullReferenceException();
        }

        _server = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream,
            ProtocolType.Tcp);
        _server.Bind(_ipEndPoint);
        _server.Listen(100);

        _handler = await _server.AcceptAsync();
    }

    public Task StartListeningAsync()
    {
        new Thread(StartListeningThread).Start();

        return Task.CompletedTask;
    }

    public async Task<int> SendMessageAsync(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        return await _handler!.SendAsync(messageBytes, 0);
    }

    private async void StartListeningThread()
    {
        var buffer = new byte[1_024];
        var received = await _handler!.ReceiveAsync(buffer, SocketFlags.None);
        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, received);

        MessageReceivedCallback?.Invoke(this, receivedMessage);
    }
}