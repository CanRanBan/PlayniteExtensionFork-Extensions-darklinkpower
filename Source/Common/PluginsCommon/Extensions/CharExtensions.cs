namespace System
{
    public static class CharExtensions
    {
        public static bool EqualsIgnoreCase(this char char1, char char2)
        {
            return char.ToUpperInvariant(char1) == char.ToUpperInvariant(char2);
        }
    }
}