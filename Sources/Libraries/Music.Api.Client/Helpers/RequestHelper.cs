using RestSharp;

namespace MusicClient.Helpers;

public static class RequestHelper
{
    public static void AddFile(this RestRequest request, string fileName)
    {
        request.AddFile("File", fileName);
    }
}