using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace FluentEmail.MailPace;

public static class StreamExtensions
{
    public static string ConvertToBase64(this Stream stream)
    {
        if (stream is MemoryStream memoryStream)
        {
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        var bytes = new Byte[(int)stream.Length];

        stream.Seek(0, SeekOrigin.Begin);
        // ReSharper disable once MustUseReturnValue
        stream.ReadExactly(bytes, 0, (int)stream.Length);

        return Convert.ToBase64String(bytes);
    }

#if NETSTANDARD
    private static void ReadExactly(this Stream s, byte[] buffer, int offset, int count)
    {
        _ = s.ReadAtLeastCore(buffer.AsSpan(offset, count), count, throwOnEndOfStream: true);
    }

    private static int Read(this Stream s, Span<byte> buffer)
    {
        byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            int numRead = s.Read(sharedBuffer, 0, buffer.Length);
            if ((uint)numRead > (uint)buffer.Length)
            {
                throw new IOException("IO_StreamTooLong");
            }

            new ReadOnlySpan<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
            return numRead;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sharedBuffer);
        }
    }

    private static int ReadAtLeastCore(this Stream s, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
    {
        Debug.Assert(minimumBytes <= buffer.Length);

        int totalRead = 0;
        while (totalRead < minimumBytes)
        {
            int read = s.Read(buffer.Slice(totalRead));
            if (read == 0)
            {
                if (throwOnEndOfStream)
                {
                    throw new EndOfStreamException();
                }

                return totalRead;
            }

            totalRead += read;
        }

        return totalRead;
    }
#endif
}