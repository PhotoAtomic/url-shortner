using System.Text;

namespace api.Utilities
{
    public class UlongEncoding
    {

        private static readonly string[] symbols = new string[] // these could be also syllables: output will be very funny and easy to write
            { "a", "b", "c", "d",
              "e", "f", "g", "h",
              "i", "j", "k", "l",
              "m", "n", "o", "p"};

        public static string ToSpecialBase16(ulong number, string? prefix=null)
        {
            var encodedNibbles = ToNibbleArray(number).Reverse().SkipWhile(n => n == 0).Select(n => symbols[n]);
            StringBuilder stringBuilder = new(); 
            if(prefix is not null)
            {
                stringBuilder.Append(prefix);
                stringBuilder.Append("-");
            }
            stringBuilder.AppendJoin(String.Empty, encodedNibbles);
            return stringBuilder.ToString();
        }


        public static IEnumerable<byte> ToNibbleArray(ulong number)
        {
            foreach (var @byte in BitConverter.GetBytes(number))
            {
                yield return (byte)(@byte & 0x0F);
                yield return (byte)((@byte & 0xF0) >> 4);
            }
        }
    }
}
