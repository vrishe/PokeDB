using System;
using System.IO;

namespace PokeDB.Insfrastructure
{
    static class IOExtensions
    {
        public static Stream WriteTo(this Stream stream, string location)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("Can't read the stream given.", nameof(stream));
            }
            var buffer = new byte[Math.Min(stream.Length, 16384)];

            Directory.CreateDirectory(
                Path.GetDirectoryName(location));

            using (var output = File.Open(location, FileMode.Create))
            {
                int countRead;
                while((countRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, countRead);
                }
            }
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }
    }
}