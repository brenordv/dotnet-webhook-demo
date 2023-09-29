echo Starting API server...
start dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Server

timeout 10
echo Starting the Subscriber...
start dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Cli.Subscribe --configuration Debug -- 8080
start dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Cli.Subscribe --configuration Release -- 8081


timeout 10
echo Starting the message sender...
start dotnet run --project .\server\Raccoon.Ninja.WebHook.Server\Raccoon.Ninja.WebHook.Cli.Sender