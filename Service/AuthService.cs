namespace Chatbot_Dev.Service
{
    public class AuthService
    {
        public static string[] Options = { "Sign Up", "Login" };

        public static string HandleAuth(string choice)
        {
            return choice == "Sign Up"
                ? "📝 Please enter your details to sign up."
                : "🔐 Please enter your login credentials.";
        }
    }
}
