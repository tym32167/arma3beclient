namespace Arma3BE.Server.Recognizers.Core
{
    public class GUIDValidator
    {
        public static bool Validate(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (value.Length != 32) return false;

            return true;
        }
    }
}