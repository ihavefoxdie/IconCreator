using SkiaSharp;

namespace ToIcon.Library;

/// <summary>
/// Provides helper methods for imaging
/// </summary>
public static class IconHelperCrossplatform
{
    /// <summary>
    /// Converts a PNG image to an icon (ico)
    /// </summary>
    /// <param name="input">The input stream</param>
    /// <param name="output">The output stream</param>
    /// <param name="size">Needs to be a factor of 2 (16x16 px by default)</param>
    /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
    /// <returns>Wether or not the icon was succesfully generated</returns>
    public static bool ConvertToIcon(byte[] input, Stream output, int size = 16, bool preserveAspectRatio = false)
    {
        SKBitmap inputBitmap = SKBitmap.Decode(input);

        if (inputBitmap == null)
        {
            throw new NullReferenceException("The input stream is null!");
        }

        double width = size, height = size;

        if (preserveAspectRatio)
        {
            if (inputBitmap.Width > inputBitmap.Height)
            {
                height = (double)inputBitmap.Height / inputBitmap.Width * size;
            }
            else
            {
                width = (double)inputBitmap.Width / inputBitmap.Height * size;
            }
        }


        SKImageInfo imageInfo = new((int)width, (int)height);
        SKBitmap newBitmap = inputBitmap.Resize(imageInfo, new SKSamplingOptions(16));

        if (newBitmap == null)
        {
            throw new NullReferenceException("Failed to resize the bitmap created from the input stream!");
        }

        using MemoryStream memoryStream = new();
        newBitmap.Encode(memoryStream, SKEncodedImageFormat.Png, 100);

        var iconWriter = new BinaryWriter(output);
        if (output == null || iconWriter == null)
        {
            throw new NullReferenceException("The output stream is likely null!");
        }

        // 0-1 reserved, 0
        iconWriter.Write((byte)0);
        iconWriter.Write((byte)0);

        // 2-3 image type, 1 = icon, 2 = cursor
        iconWriter.Write((short)1);

        // 4-5 number of images
        iconWriter.Write((short)1);

        // image entry 1
        // 0 image width
        iconWriter.Write((byte)width);
        // 1 image height
        iconWriter.Write((byte)height);

        // 2 number of colors
        iconWriter.Write((byte)0);

        // 3 reserved
        iconWriter.Write((byte)0);

        // 4-5 color planes
        iconWriter.Write((short)0);

        // 6-7 bits per pixel
        iconWriter.Write((short)32);

        // 8-11 size of image data
        iconWriter.Write((int)memoryStream.Length);

        // 12-15 offset of image data
        iconWriter.Write((int)(6 + 16));

        // write image data
        // png data must contain the whole png data file
        iconWriter.Write(memoryStream.ToArray());

        iconWriter.Flush();

        return true;
    }

    /// <summary>
    /// Converts a PNG image to an icon (ico)
    /// </summary>
    /// <param name="inputPath">The input path</param>
    /// <param name="outputPath">The output path</param>
    /// <param name="size">Needs to be a factor of 2 (16x16 px by default)</param>
    /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
    /// <returns>Wether or not the icon was succesfully generated</returns>
    public static bool ConvertToIcon(string inputPath, string outputPath, int size = 16, bool preserveAspectRatio = false)
    {
        using FileStream inputStream = new(inputPath, FileMode.Open);
        using FileStream outputStream = new(outputPath, FileMode.OpenOrCreate);
        using MemoryStream memoryStream = new();
        inputStream.CopyTo(memoryStream);
        byte[] imageBytes = memoryStream.ToArray();
        return ConvertToIcon(imageBytes, outputStream, size, preserveAspectRatio);
    }

    // /// <summary>
    // /// Converts a PNG image to an icon (ico)
    // /// </summary>
    // /// <param name="inputPath">Image object</param>
    // /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
    // /// <returns>ico byte array / null for error</returns>
    // public static byte[]? ConvertToIcon(Image image, bool preserveAspectRatio = false)
    // {
    //     MemoryStream inputStream = new();
    //     image.Save(inputStream, System.Drawing.Imaging.ImageFormat.Png);
    //     inputStream.Seek(0, SeekOrigin.Begin);
    //     MemoryStream outputStream = new MemoryStream();
    //     int size = image.Size.Width;
    //     if (!ConvertToIcon(inputStream, outputStream, size, preserveAspectRatio))
    //     {
    //         return null;
    //     }
    //     return outputStream.ToArray();
    // }

}
