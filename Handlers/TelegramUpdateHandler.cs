
using Chatbot_Dev.Service;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Chatbot_Dev.Handlers;


public class TelegramUpdateHandler
{


    private static Dictionary<long, string> userState = new();

    public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update)
    {
        var message = update.Message;
        if (message == null || message.Text == null) return;

        var chatId = message.Chat.Id;

        if (!userState.ContainsKey(chatId))
        {
            userState[chatId] = "language";
            await bot.SendTextMessageAsync(chatId, "Please select a language:",
                replyMarkup: new ReplyKeyboardMarkup(LanguageService.Languages.Select(x => new[] { new KeyboardButton(x) })) { ResizeKeyboard = true });
            return;
        }

        var state = userState[chatId];

        switch (state)
        {
            case "language":
                var langCode = LanguageService.GetLanguageCode(message.Text);
                userState[chatId] = "auth";
                await bot.SendTextMessageAsync(chatId, "Do you want to sign up or login?",
                    replyMarkup: new ReplyKeyboardMarkup(AuthService.Options.Select(x => new[] { new KeyboardButton(x) })) { ResizeKeyboard = true });
                break;

            case "auth":
                await bot.SendTextMessageAsync(chatId, AuthService.HandleAuth(message.Text));
                userState[chatId] = "service";
                await bot.SendTextMessageAsync(chatId, "Select a service:",
                    replyMarkup: new ReplyKeyboardMarkup(new[] {
                        new[] { new KeyboardButton("Web Development") },
                        new[] { new KeyboardButton("Graphic Design") },
                        new[] { new KeyboardButton("App Development") }
                    })
                    { ResizeKeyboard = true });
                break;

            case "service":
                string response = message.Text switch
                {
                    "Web Development" => WebDevService.GetResponse(),
                    "Graphic Design" => GraphicDesignService.GetResponse(),
                    "App Development" => AppDevService.GetResponse(),
                    _ => "❌ Unknown service."
                };
                await bot.SendTextMessageAsync(chatId, response);
                break;
        }
    }
}





