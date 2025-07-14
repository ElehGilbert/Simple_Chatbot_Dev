// Program.cs
//using Chatbot_Dev.Handlers;
using Telegram.Bot;
using TelegramBot.Handlers;


var builder = WebApplication.CreateBuilder(args);


// ⬅️ This line makes sure appsettings.json is read
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// Load bot token from config
var botToken = builder.Configuration["TelegramBotToken"];
var botClient = new TelegramBotClient(botToken);







var app = builder.Build();

app.MapGet("/", () => "Telegram Bot is running");

// Background polling service
var cancellationToken = new CancellationTokenSource();
_ = Task.Run(async () =>
{
    var me = await botClient.GetMeAsync();
    Console.WriteLine($"🤖 Bot {me.Username} is up!");

    botClient.StartReceiving(
//        async (client, update, token) => await TelegramUpdateHandler.HandleUpdateAsync(client, update),
//        (client, exception, token) =>
//        {
//            Console.WriteLine($"❌ Error: {exception.Message}");
//            return Task.CompletedTask;
//        },
//        cancellationToken: cancellationToken.Token);
//});
 async (client, update, token) =>
 {
     await TelegramUpdateHandler.HandleUpdateAsync(client, update);
 },
    (client, exception, token) =>
    {
        Console.WriteLine($"❌ Error: {exception.Message}");
        return Task.CompletedTask;
    },
    cancellationToken: cancellationToken.Token);
});

app.Run();
