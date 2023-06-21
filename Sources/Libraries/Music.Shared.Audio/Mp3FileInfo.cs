using Audio.FileFormats.Mp3;
using Audio.Other;
using Audio.Wave.WaveFormats;

namespace Music.Shared.Audio;

public class Mp3FileInfo
{
    public Mp3FileInfo(Stream audioStream)
    {
        var reader = new Mp3FileReader(audioStream);
        WaveFormat = reader.WaveFormat;
        BitsPerSample = WaveFormat.BitsPerSample;
        SampleRate = WaveFormat.SampleRate;
        Channels = WaveFormat.Channels;
        BytesCount = reader.Length;
        Duration = reader.TotalTime;
        var frames = new List<Mp3FrameLight>();
        var number = 0;
        while (reader.ReadNextFrame() is { } frame)
        {
            if(BitRate == default)
                BitRate = frame.BitRate;
            if (FrameLength == default)
                FrameLength = frame.FrameLength;
            frame.Number = number++;
            frames.Add(frame.ToLight());
        }
        Frames = frames.ToArray();
        reader.Dispose();
    }
    
    public Mp3FileInfo(string path)
    {
        Path = path;
        var reader = new Mp3FileReader(Path);
        WaveFormat = reader.WaveFormat;
        BitsPerSample = WaveFormat.BitsPerSample;
        SampleRate = WaveFormat.SampleRate;
        Channels = WaveFormat.Channels;
        BytesCount = reader.Length;
        Duration = reader.TotalTime;
        var frames = new List<Mp3FrameLight>();
        var number = 0;
        while (reader.ReadNextFrame() is { } frame)
        {
            if(BitRate == default)
                BitRate = frame.BitRate;
            if (FrameLength == default)
                FrameLength = frame.FrameLength;
            frame.Number = number++;
            frames.Add(frame.ToLight());
        }
        Frames = frames.ToArray();
        reader.Dispose();
    }

    public int Channels { get; }
    public int BitRate { get; } 
    public int FrameLength { get; } 
    public int SampleRate { get; } 
    public int BitsPerSample { get; } 
    public string Path { get; }
    public TimeSpan Duration { get; }
    public long FramesCount => Frames.Length;
    public long BytesCount { get; }
    public Mp3FrameLight[] Frames { get; }
    public WaveFormat WaveFormat { get; }
}