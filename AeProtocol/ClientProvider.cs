using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AeProtocol;

public class ClientProvider
{
    private readonly IPEndPoint? _ipEndPoint;

    private Socket? _client;

    public event EventHandler<string>? MessageReceivedCallback;

    public ClientProvider(int port)
    {
        var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostEntry.AddressList[0];

        _ipEndPoint = new IPEndPoint(ipAddress, port);
        _client = null;
    }

    public async Task ConnectAsync()
    {
        if (_ipEndPoint == null)
        {
            throw new NullReferenceException();
        }

        _client = new Socket(_ipEndPoint.AddressFamily, SocketType.Stream,
            ProtocolType.Tcp);

        await _client.ConnectAsync(_ipEndPoint);
    }

    public Task StartListeningAsync()
    {
        new Thread(StartListeningThread).Start();

        return Task.CompletedTask;
    }

    public async Task SendMessageAsync(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _client!.SendAsync(messageBytes, SocketFlags.None);
    }

    public Task DisposeAsync()
    {
        if (_client == null)
        {
            throw new NullReferenceException();
        }

        _client.Shutdown(SocketShutdown.Both);
        _client.Dispose();

        return Task.CompletedTask;
    }

    private async void StartListeningThread()
    {
        while (true)
        {
            var buffer = new byte[1_024];
            var received = await _client!.ReceiveAsync(buffer, SocketFlags.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, received);
            
            MessageReceivedCallback?.Invoke(this, receivedMessage);
        }
    }
}