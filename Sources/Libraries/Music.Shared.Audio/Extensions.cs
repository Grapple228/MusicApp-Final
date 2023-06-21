using Audio.FileFormats.Mp3;

namespace Music.Shared.Audio;

public static class Extensions
{
    public static IEnumerable<Mp3FrameLight> GetFrames(this Mp3FileInfo fileInfo, IEnumerable<long> indexes) =>
        fileInfo.Frames.Where(x => indexes.Contains(x.Number)).ToArray();
    
    public static IEnumerable<Mp3FrameLight> GetFrames(this Mp3FileInfo fileInfo) =>
        fileInfo.Frames.DistinctBy(x => x.Number).ToArray();

    public static Mp3FrameLight ToLight(this Mp3Frame frame) => new(frame.Number, frame.FrameLength, frame.FileOffset, frame.RawData);

    // TODO FrameNumber
    public static long GetFrameNumber(this IMp3Frame frame, long totalLength, long position) => 0;
}