using System.Linq;

namespace MioMatrix.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToDebugString(this byte[] bytes)
        {
            return string.Join(" ", bytes.Select(x => "0x" + x.ToString("X2")));
        } 
    }
}