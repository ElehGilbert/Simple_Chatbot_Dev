    using System.Text.RegularExpressions;


namespace Chatbot_Dev.Validations
{


    public class Validations
    {



        // Validate phone number format: starts with + and has 8 to 15 digits
        public static bool IsValidPhoneNumber(string phone)
        {
            return Regex.IsMatch(phone, @"^\+\d{8,15}$");
        }

        // Basic email format validation
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                
                RegexOptions.IgnoreCase);
        }


    }
}



