using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading;

public class dWebHook : IDisposable
{
    private readonly WebClient dWebClient;

    public string WebHook { get; set; }

    public dWebHook()
    {
        dWebClient = new WebClient();
    }

    public void SendMessage(string msgSend)
    {
        var discordValues = new NameValueCollection
        {
            { "content", msgSend }
        };

        try
        {
            dWebClient.UploadValues(WebHook, discordValues);
            Console.WriteLine($"Message sent: {msgSend}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        dWebClient.Dispose();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.Write("HitFusion's webhook spammer");
        Console.Write("webhook URL: ");
        string webhookUrl = Console.ReadLine();

        Console.Write("message content:");
        string message = Console.ReadLine();

        Console.Write("how many times repeat:");
        if (!int.TryParse(Console.ReadLine(), out int repeatCount) || repeatCount <= 0)
        {
            repeatCount = 1;
        }

        Console.Write("number of threads(2 recommended): ");
        if (!int.TryParse(Console.ReadLine(), out int threadCount) || threadCount <= 0)
        {
            threadCount = 1;
        }

        Console.WriteLine($"\nsending {threadCount} threads...");

        Thread[] threads = new Thread[threadCount];
        int messagesPerThread = repeatCount / threadCount;
        int remainingMessages = repeatCount % threadCount;

        for (int i = 0; i < threadCount; i++)
        {
            int countForThisThread = messagesPerThread + (i < remainingMessages ? 1 : 0);
            threads[i] = new Thread(() => SendMessages(webhookUrl, message, countForThisThread));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("\nsent");
    }

    static void SendMessages(string webhookUrl, string message, int count)
    {
        using (var webhook = new dWebHook())
        {
            webhook.WebHook = webhookUrl;

            for (int i = 0; i < count; i++)
            {
                webhook.SendMessage(message);
                Thread.Sleep(500); 
            }
        }
    }
}
