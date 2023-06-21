namespace Audio.FileFormats.Mp3;

public record Mp3FrameLight(int Number, int FrameLength, long FileOffset, byte[] RawData) : IMp3Frame;