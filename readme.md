# .net Webhook demo
This is a simple demo of how to create a webhook in a minimal API. Granted it is a very naive implementation, but if 
you don't know where to start, this might be a good option.

# Projects

- [Api Code](./server/Raccoon.Ninja.WebHook.Server/Raccoon.Ninja.WebHook.Server/Program.cs)
- [Client Subscriber](./server/Raccoon.Ninja.WebHook.Server/Raccoon.Ninja.WebHook.Cli.Subscribe/Program.cs)
- [Client Sender](./server/Raccoon.Ninja.WebHook.Server/Raccoon.Ninja.WebHook.Cli.Sender/Program.cs)

# What is going to happen
The clients will subscribe to receive every new message posted to the server. The sender will post a message every
5 seconds and that message will be forwarded to all subscribers.

The message format:
```json
{
  "createdAt": "2021-10-10T00:00:00.0000000Z",
  "quote": "This is a quote",
  "luckyNumbers": [1,2,3,4,5,6]
}
```

# How to run
To get everything up and running quickly, you can execute the `run-server-test.bat` on the root folder.
Or you could run the commands separately:

## Running the server
```shell
dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Server
```

## Running two subscriber clients
```shell
dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Cli.Subscribe --configuration Debug -- 8080
dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Cli.Subscribe --configuration Release -- 8081
```

## Running the sender client
```shell
dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Cli.Sender
```

# Next Steps
1. Implement another API server using SignalR
2. Implement react and angular clients to consume that API