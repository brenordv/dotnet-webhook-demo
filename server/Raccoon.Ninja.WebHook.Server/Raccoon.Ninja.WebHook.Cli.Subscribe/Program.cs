using System.Net;
using System.Text;
using System.Text.Json;

Console.WriteLine("I'm the subscriber. I receive the messages pushed to the server");
Console.WriteLine("###############################################################");

var messageCount = 0;
const string apiServerUrl = "http://localhost:5058"; // Replace with your API server URL
var listeningUrl = $"http://localhost:{args[0]}/"; // Replace with your desired listening URL

Console.WriteLine($"Going to listen on: {listeningUrl}");

// Step 1: Subscribe to the API server
using var httpClient = new HttpClient();
var subscription = new { Url = listeningUrl };
var jsonPayload = JsonSerializer.Serialize(subscription);
var response = await httpClient.PostAsync($"{apiServerUrl}/subscribe",
    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

if (response.IsSuccessStatusCode)
{
    Console.WriteLine("Successfully subscribed.");
}
else
{
    Console.WriteLine($"Failed to subscribe. Status code: {response.StatusCode}");
    return;
}

// Step 2: Start HTTP Listener
using var listener = new HttpListener();
listener.Prefixes.Add(listeningUrl);
listener.Start();
Console.WriteLine($"Listening on {listeningUrl}");

while (messageCount < 30)
{
    var context = await listener.GetContextAsync();
    var request = context.Request;

    if (request.HttpMethod == "POST")
    {
        using var reader = new StreamReader(request.InputStream);
        var requestBody = await reader.ReadToEndAsync();
        var message = JsonSerializer.Deserialize<WebhookMessage>(requestBody);

        // Increment message count and display message
        messageCount++;
        Console.WriteLine($"Received message {messageCount:2D}: {message.Quote} at {message.CreatedAt}");

        // Send 200 OK response
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Received"));
        context.Response.Close();
    }
}

// Step 4: Unsubscribe and exit
response = await httpClient.PostAsync($"{apiServerUrl}/unsubscribe",
    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

Console.WriteLine(response.IsSuccessStatusCode
    ? "Successfully unsubscribed."
    : $"Failed to unsubscribe. Status code: {response.StatusCode}");

listener.Stop();
Console.WriteLine("Exiting application.");

record WebhookMessage(DateTime CreatedAt, string Quote, int[] LuckyNumbers);