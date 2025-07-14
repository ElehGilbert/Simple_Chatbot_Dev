namespace Chatbot_Dev.Service
{
    public class LanguageService
    {
        public static string[] Languages = { "🇬🇧 English", "🇫🇷 Français" };

        public static string GetLanguageCode(string input)
        {
            return input.Contains("Français") ? "fr" : "en";
        }
    }
}
