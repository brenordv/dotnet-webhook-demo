using System.Text;
using Newtonsoft.Json;

Console.WriteLine("I'm the API Server. I manage this whole thing.");
Console.WriteLine("##############################################");


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// List to store subscribed client URLs
List<string> subscribedClients = new();

app.MapPost("/webhook", HandleWebHook);

app.MapPost("/subscribe", Subscribe);

app.MapPost("/unsubscribe", Unsubscribe);

app.Run();

async Task HandleWebHook(HttpContext context)
{
    try
    {
        // Deserialize incoming message
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var incomingMessage = JsonConvert.DeserializeObject<WebhookMessage>(requestBody);

        // Log the incoming message
        Console.WriteLine($"Received a message at {incomingMessage?.CreatedAt}: {incomingMessage?.Quote}");

        // Forward the message to all subscribed clients
        var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient();
        
        foreach (var subscriber in subscribedClients)
        {
            var jsonPayload = JsonConvert.SerializeObject(incomingMessage);
            var response = await client.PostAsync(subscriber,
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to forward message to {subscriber}. Status code: {response.StatusCode}");
            }
        }

        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Received");
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync($"An error occurred: {ex.Message}");
    }
}

async Task Subscribe(HttpContext context)
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var subscription = JsonConvert.DeserializeObject<Subscription>(requestBody);

    if (subscription?.Url != null && !subscribedClients.Contains(subscription.Url))
    {
        subscribedClients.Add(subscription.Url);
    }
    
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("Subscribed");
}

async Task Unsubscribe(HttpContext context)
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var subscription = JsonConvert.DeserializeObject<Subscription>(requestBody);

    if (subscription?.Url != null)
        subscribedClients.Remove(subscription.Url);
    
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("Unsubscribed");
}

public record WebhookMessage(DateTime CreatedAt, string Quote, IList<int> LuckyNumbers);
public record Subscription(string Url);