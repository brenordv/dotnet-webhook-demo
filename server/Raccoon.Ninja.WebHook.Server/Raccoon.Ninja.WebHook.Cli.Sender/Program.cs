using System.Text;
using System.Text.Json;

Console.WriteLine("######################################################################");
Console.WriteLine("# I'm the sender. I send messages to the API server every 5 seconds. #");
Console.WriteLine("######################################################################");

const string apiServerUrl = "http://localhost:5058/webhook";
var executableLocation = AppDomain.CurrentDomain.BaseDirectory;
var fullPath = Path.Combine(executableLocation, "quotes.txt");
var quotes = File.ReadAllLines(fullPath);
var random = new Random();

using var httpClient = new HttpClient();

while (true)
{
    var createdAt = DateTime.Now;
    var quote = quotes[random.Next(quotes.Length)];
    var luckyNumbers = Enumerable.Range(1, 60).OrderBy(x => random.Next()).Take(6).ToList();

    var message = new WebhookMessage(createdAt, quote, luckyNumbers);

    var jsonPayload = JsonSerializer.Serialize(message);
    var response = await httpClient.PostAsync(apiServerUrl,
        new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

    Console.WriteLine(response.IsSuccessStatusCode
        ? $"Successfully sent message: {quote}"
        : $"Failed to send message. Status code: {response.StatusCode}");

    await Task.Delay(TimeSpan.FromSeconds(5));
}

public record WebhookMessage(DateTime CreatedAt, string Quote, IList<int> LuckyNumbers);