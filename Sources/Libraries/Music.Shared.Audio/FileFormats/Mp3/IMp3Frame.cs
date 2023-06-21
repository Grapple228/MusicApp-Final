namespace Audio.FileFormats.Mp3;

public interface IMp3Frame
{
    public int Number { get; }
    public int FrameLength{ get; }
    public long FileOffset{ get; }
    public byte[] RawData{ get; }
}