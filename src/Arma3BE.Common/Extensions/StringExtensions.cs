namespace Arma3BEClient.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsGuid(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (str.Length != 32) return false;
            foreach (var c in str)
            {
                if (!char.IsLetterOrDigit(c)) return false;
                if (char.IsLetter(c) && char.IsUpper(c)) return false;
            }
            return true;
        }
    }
}