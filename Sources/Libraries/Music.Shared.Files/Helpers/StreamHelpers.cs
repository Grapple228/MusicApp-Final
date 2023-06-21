using System.Drawing;
using System.Drawing.Imaging;

namespace Music.Shared.Files.Helpers;

public static class StreamHelpers
{
    public static Stream ToStream(this Image image, ImageFormat format) {
        var stream = new MemoryStream();
        image.Save(stream, format);
        stream.Position = 0;
        return stream;
    }
}