namespace Audio.FileFormats.Mp3;

/// <summary>
/// Represents an MP3 Frame
/// </summary>
public class Mp3Frame : IMp3Frame
{
    private static readonly int[,,] BitRates = {
        {
            // MPEG Version 1
            { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 }, // Layer 1
            { 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384 }, // Layer 2
            { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 }, // Layer 3
        },
        {
            // MPEG Version 2 & 2.5
            { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256 }, // Layer 1
            { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 }, // Layer 2 
            { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 }, // Layer 3 (same as layer 2)
        }
    };

    private static readonly int[,] SamplesPerFrame = new int[,] {
        {   // MPEG Version 1
            384,    // Layer1
            1152,   // Layer2
            1152    // Layer3
        },
        {   // MPEG Version 2 & 2.5
            384,    // Layer1
            1152,   // Layer2
            576     // Layer3
        }
    };

    protected static readonly int[] SampleRatesVersion1 = {44100, 48000, 32000};
    protected static readonly int[] SampleRatesVersion2 = {22050, 24000, 16000};
    protected static readonly int[] SampleRatesVersion25 = {11025, 12000, 8000};
    public int Number { get; set; }
    //private short crc;
    protected const int MaxFrameLength = 16*1024;

    /// <summary>
    /// Reads an MP3 frame from a stream
    /// </summary>
    /// <param name="input">input stream</param>
    /// <returns>A valid MP3 frame, or null if none found</returns>
    public static Mp3Frame LoadFromStream(Stream input)
    {
        return LoadFromStream(input, true);
    }

    /// <summary>Reads an MP3Frame from a stream</summary>
    /// <remarks>http://mpgedit.org/mpgedit/mpeg_format/mpeghdr.htm has some good info
    /// also see http://www.codeproject.com/KB/audio-video/mpegaudioinfo.aspx
    /// </remarks>
    /// <returns>A valid MP3 frame, or null if none found</returns>
    public static Mp3Frame LoadFromStream(Stream input, bool readData)
    {
        var frame = new Mp3Frame
        {
            FileOffset = input.Position
        };
        var headerBytes = new byte[4];
        var bytesRead = input.Read(headerBytes, 0, headerBytes.Length);
        if (bytesRead < headerBytes.Length)
        {
            // reached end of stream, no more MP3 frames
            return null;
        }
        while (!IsValidHeader(headerBytes, frame))
        {
            // shift down by one and try again
            headerBytes[0] = headerBytes[1];
            headerBytes[1] = headerBytes[2];
            headerBytes[2] = headerBytes[3];
            bytesRead = input.Read(headerBytes, 3, 1);
            if (bytesRead < 1)
            {
                return null;
            }
            frame.FileOffset++;
        }
        /* no longer read the CRC since we include this in framelengthbytes
        if (this.crcPresent)
            this.crc = reader.ReadInt16();*/

        int bytesRequired = frame.FrameLength - 4;
        if (readData)
        {
            frame.RawData = new byte[frame.FrameLength];
            Array.Copy(headerBytes, frame.RawData, 4);
            bytesRead = input.Read(frame.RawData, 4, bytesRequired);
            if (bytesRead < bytesRequired)
            {
                // TODO: could have an option to suppress this, although it does indicate a corrupt file
                // for now, caller should handle this exception
                throw new EndOfStreamException("Unexpected end of stream before frame complete");
            }
        }
        else
        {
            // n.b. readData should not be false if input stream does not support seeking
            input.Position += bytesRequired;
        }

        return frame;
    }


    /// <summary>
    /// Constructs an MP3 frame
    /// </summary>
    private Mp3Frame()
    {

    }

    /// <summary>
    /// checks if the four bytes represent a valid header,
    /// if they are, will parse the values into Mp3Frame
    /// </summary>
    protected static bool IsValidHeader(byte[] headerBytes, Mp3Frame frame)
    {
        if ((headerBytes[0] != 0xFF) || ((headerBytes[1] & 0xE0) != 0xE0)) return false;
        // TODO: could do with a bitstream class here
        frame.MpegVersion = (MpegVersion) ((headerBytes[1] & 0x18) >> 3);
        if (frame.MpegVersion == MpegVersion.Reserved)
        {
            //throw new FormatException("Unsupported MPEG Version");
            return false;
        }

        frame.MpegLayer = (MpegLayer) ((headerBytes[1] & 0x06) >> 1);

        if (frame.MpegLayer == MpegLayer.Reserved)
        {
            return false;
        }
        var layerIndex = frame.MpegLayer switch
        {
            MpegLayer.Layer1 => 0,
            MpegLayer.Layer2 => 1,
            _ => 2
        };
        frame.CrcPresent = (headerBytes[1] & 0x01) == 0x00;
        frame.BitRateIndex = (headerBytes[2] & 0xF0) >> 4;
        if (frame.BitRateIndex == 15)
        {
            // invalid index
            return false;
        }
        var versionIndex = frame.MpegVersion == MpegVersion.Version1 ? 0 : 1;
        frame.BitRate = BitRates[versionIndex, layerIndex, frame.BitRateIndex]*1000;
        if (frame.BitRate == 0)
        {
            return false;
        }
        var sampleFrequencyIndex = (headerBytes[2] & 0x0C) >> 2;
        if (sampleFrequencyIndex == 3)
        {
            return false;
        }

        frame.SampleRate = frame.MpegVersion switch
        {
            MpegVersion.Version1 => SampleRatesVersion1[sampleFrequencyIndex],
            MpegVersion.Version2 => SampleRatesVersion2[sampleFrequencyIndex],
            _ => SampleRatesVersion25[sampleFrequencyIndex]
        };

        var padding = (headerBytes[2] & 0x02) == 0x02;
        var privateBit = (headerBytes[2] & 0x01) == 0x01;
        frame.ChannelMode = (ChannelMode) ((headerBytes[3] & 0xC0) >> 6);
        frame.ChannelExtension = (headerBytes[3] & 0x30) >> 4;
        if (frame.ChannelExtension != 0 && frame.ChannelMode != ChannelMode.JointStereo)
        {
            return false;
        }


        frame.Copyright = (headerBytes[3] & 0x08) == 0x08;
        var original = (headerBytes[3] & 0x04) == 0x04;
        var emphasis = (headerBytes[3] & 0x03);

        var nPadding = padding ? 1 : 0;

        frame.SampleCount = SamplesPerFrame[versionIndex, layerIndex];
        var coefficient = frame.SampleCount/8;
        if (frame.MpegLayer == MpegLayer.Layer1)
        {
            frame.FrameLength = (coefficient*frame.BitRate/frame.SampleRate + nPadding)*4;
        }
        else
        {
            frame.FrameLength = (coefficient*frame.BitRate)/frame.SampleRate + nPadding;
        }

        return frame.FrameLength <= MaxFrameLength;
    }

    /// <summary>
    /// Sample rate of this frame
    /// </summary>
    public int SampleRate { get; private set; }

    /// <summary>
    /// Frame length in bytes
    /// </summary>
    public int FrameLength { get; private set; }

    /// <summary>
    /// Bit Rate
    /// </summary>
    public int BitRate { get; private set; }

    /// <summary>
    /// Raw frame data (includes header bytes)
    /// </summary>
    public byte[] RawData { get; private set; }

    /// <summary>
    /// MPEG Version
    /// </summary>
    public MpegVersion MpegVersion { get; private set; }

    /// <summary>
    /// MPEG Layer
    /// </summary>
    public MpegLayer MpegLayer { get; private set; }

    /// <summary>
    /// Channel Mode
    /// </summary>
    public ChannelMode ChannelMode { get; private set; }

    /// <summary>
    /// The number of samples in this frame
    /// </summary>
    public int SampleCount { get; private set; }

    /// <summary>
    /// The channel extension bits
    /// </summary>
    public int ChannelExtension { get; private set; }

    /// <summary>
    /// The bitrate index (directly from the header)
    /// </summary>
    public int BitRateIndex { get; private set; }

    /// <summary>
    /// Whether the Copyright bit is set
    /// </summary>
    public bool Copyright { get; private set; }

    /// <summary>
    /// Whether a CRC is present
    /// </summary>
    public bool CrcPresent { get; private set; }


    /// <summary>
    /// Not part of the MP3 frame itself - indicates where in the stream we found this header
    /// </summary>
    public long FileOffset { get; private set; }
}