using AeProtocol;

internal abstract class Client
{
    public static async Task Main()
    {
        Console.WriteLine("Connecting client...");
        var server = new ClientProvider(1100);

        await server.ConnectAsync();
        await server.StartListeningAsync();

        Console.WriteLine("Client connected. Listening to messages...");
        
        server.MessageReceivedCallback += ReceiveMessageAsync;

        Console.WriteLine("Client waiting for message input...");
        var message = Console.ReadLine() ?? string.Empty;
        await server.SendMessageAsync(message);
        
        Console.WriteLine("Client message sent.");
    }

    private static void ReceiveMessageAsync(object? sender, string e)
    {
        Console.WriteLine($"Client message received: {e}");
    }
}