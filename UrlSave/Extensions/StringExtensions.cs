namespace UrlSave.Extensions
{
    public static class StringExtensions
    {
        public static long ToLong(this string value) 
        {
            string digits = new(value.Where(char.IsDigit).ToArray());

            if (long.TryParse(digits, out long priceValue))
            {
                return priceValue;
            }
            else
            {
                return 0;
            }
        }
    }
}
