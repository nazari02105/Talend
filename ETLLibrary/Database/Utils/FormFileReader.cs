using System;
using System.IO;
using System.Linq;

namespace ETLLibrary.Database.Utils
{
    public static class FormFileReader
    {
        public static string Read(Stream stream, long fileLength)
        {
            var bytes = new byte[fileLength];
            stream.Read(bytes);
            var content = bytes.Aggregate("", (current, b) => current + Convert.ToChar((byte) b));
            return content;
        }

        
    }
}