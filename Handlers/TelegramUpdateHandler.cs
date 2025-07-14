using Chatbot_Dev.Entities;
using Chatbot_Dev.Service;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Chatbot_Dev.Validations;
//using TelegramBot.Services;

namespace TelegramBot.Handlers;

public class TelegramUpdateHandler
{
    // Track user states: state + step of multi-step inputs
    private static Dictionary<long, UserSession> userSessions = new();

    private static Random random = new();

    public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update)
    {
        var message = update.Message;
        if (message == null || message.Text == null) return;

        var chatId = message.Chat.Id;
        var text = message.Text.Trim();

        if (!userSessions.ContainsKey(chatId))
        {
            // New user, start language selection
            userSessions[chatId] = new UserSession { State = "language" };

            await bot.SendTextMessageAsync(chatId, "Please select a language:",
                replyMarkup: new ReplyKeyboardMarkup(LanguageService.Languages.Select(x => new[] { new KeyboardButton(x) }))
                {
                    ResizeKeyboard = true
                });
            return;
        }

        var session = userSessions[chatId];

        switch (session.State)
        {
            case "language":
                session.Language = LanguageService.GetLanguageCode(text);
                session.State = "auth";
                await bot.SendTextMessageAsync(chatId, "Do you want to Sign Up or Login?",
                    replyMarkup: new ReplyKeyboardMarkup(AuthService.Options.Select(x => new[] { new KeyboardButton(x) }))
                    {
                        ResizeKeyboard = true
                    });
                break;

            case "auth":
                if (text == "Sign Up")
                {
                    session.State = "signup_firstname";
                    await bot.SendTextMessageAsync(chatId, "📝 Please enter your First Name:");
                }
                else if (text == "Login")
                {
                    session.State = "login_email";
                    await bot.SendTextMessageAsync(chatId, "🔐 Please enter your Email to login:");
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, "Please choose 'Sign Up' or 'Login'.");
                }
                break;

            #region SignUp Flow

            case "signup_firstname":
                session.SignUpData.FirstName = text;
                session.State = "signup_lastname";
                await bot.SendTextMessageAsync(chatId, "Please enter your Last Name:");
                break;

            case "signup_lastname":
                session.SignUpData.LastName = text;
                session.State = "signup_phone";
                await bot.SendTextMessageAsync(chatId, "Please enter your Phone Number with country code (e.g. +1234567890):");
                break;

            case "signup_phone":
                if (!Validations.IsValidPhoneNumber(text))
                {
                    await bot.SendTextMessageAsync(chatId, "Invalid phone number format. Please include the country code (e.g. +1234567890):");
                    return;
                }
                session.SignUpData.PhoneNumber = text;
                session.State = "signup_email";
                await bot.SendTextMessageAsync(chatId, "Please enter your Email address:");
                break;

            case "signup_email":
                if (!Validations.IsValidEmail(text))
                {
                    await bot.SendTextMessageAsync(chatId, "Invalid email format. Please enter a valid Email address:");
                    return;
                }
                session.SignUpData.Email = text;
                session.State = "signup_password";
                await bot.SendTextMessageAsync(chatId, "Please enter your Password:");
                break;

            case "signup_password":
                session.SignUpData.Password = text;
                session.State = "service";

                // Here you would save the user to a DB (not implemented)

                await bot.SendTextMessageAsync(chatId,
                    $"✅ Sign Up complete! Welcome {session.SignUpData.FirstName}.\nPlease select a service:",
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new[] { new KeyboardButton("Web Development") },
                        new[] { new KeyboardButton("Graphic Design") },
                        new[] { new KeyboardButton("App Development") }
                    })
                    {
                        ResizeKeyboard = true
                    });
                break;

            #endregion

            #region Login Flow

            case "login_email":
                if (!Validations.IsValidEmail(text))
                {
                    await bot.SendTextMessageAsync(chatId, "Invalid email format. Please enter a valid Email address:");
                    return;
                }
                session.LoginEmail = text;
                // Simulate sending code by generating a random 6-digit code and storing in session
                session.LoginCode = random.Next(100000, 999999).ToString();
                session.State = "login_code";

                // Simulate email sending (in real life you'd send the code by email)
                Console.WriteLine($"[Simulated Email] Sending code {session.LoginCode} to {session.LoginEmail}");

                await bot.SendTextMessageAsync(chatId, $"A code has been sent to your email {session.LoginEmail}. Please enter the code:");
                break;

            case "login_code":
                if (text == session.LoginCode)
                {
                    session.State = "service";
                    await bot.SendTextMessageAsync(chatId, "✅ Login successful! Please select a service:",
                        replyMarkup: new ReplyKeyboardMarkup(new[]
                        {
                            new[] { new KeyboardButton("Web Development") },
                            new[] { new KeyboardButton("Graphic Design") },
                            new[] { new KeyboardButton("App Development") }
                        })
                        {
                            ResizeKeyboard = true
                        });
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, "❌ Incorrect code. Please try again:");
                }
                break;

            #endregion

            case "service":
                string response = text switch
                {
                    "Web Development" => WebDevService.GetResponse(),
                    "Graphic Design" => GraphicDesignService.GetResponse(),
                    "App Development" => AppDevService.GetResponse(),
                    _ => "❌ Unknown service."
                };
                await bot.SendTextMessageAsync(chatId, response);
                break;

            default:
                await bot.SendTextMessageAsync(chatId, "Sorry, I didn't understand that. Please restart the conversation.");
                userSessions.Remove(chatId);
                break;
        }
    }

   
    }

   

   

