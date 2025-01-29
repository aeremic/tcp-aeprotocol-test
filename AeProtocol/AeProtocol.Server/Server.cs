using AeProtocol;

internal abstract class Server
{
    public static async Task Main()
    {
        Console.WriteLine("Connecting server...");
        var server = new ServerProvider(1100);

        await server.ConnectAsync();
        await server.StartListeningAsync();

        Console.WriteLine("Server connected. Listening to messages...");
        
        server.MessageReceivedCallback += ReceiveMessageAsync;

        Console.WriteLine("Server waiting for message input...");
        var message = Console.ReadLine() ?? string.Empty;
        await server.SendMessageAsync(message);
        
        Console.WriteLine("Server message sent.");
    }

    private static void ReceiveMessageAsync(object? sender, string e)
    {
        Console.WriteLine($"Server message received: {e}");
    }
}
