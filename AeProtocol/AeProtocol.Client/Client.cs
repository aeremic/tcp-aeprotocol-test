namespace AeProtocol.Client;

internal abstract class Client
{
    public static async Task Main()
    {
        Console.WriteLine("Connecting client...");
        var client = new ClientProvider(1100);

        await client.ConnectAsync();
        await client.StartListeningAsync(ReceiveMessageAsync);

        Console.WriteLine("Client connected. Listening to messages...");
        
        // server.MessageReceivedCallback += ReceiveMessageAsync;
        
        Console.WriteLine("Client waiting for message input...");
        var message = Console.ReadLine() ?? string.Empty;
        await client.SendMessageAsync(message);
        
        Console.WriteLine("Client message sent.");
    }

    private static void ReceiveMessageAsync(string message)
    {
        Console.WriteLine($"Client message received: {message}");
    }
    
    private static void ReceiveMessageAsync(object? sender, string e)
    {
        Console.WriteLine($"Client message received: {e}");
    }
}