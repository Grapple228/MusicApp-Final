using MusicClient.Exceptions;
using Newtonsoft.Json;
using RestSharp;

namespace MusicClient.Helpers;

public static class ResponseHelper
{
    public static T Deserialize<T>(this RestResponse response)
    {
        if (response.Content == null)
            throw new ServerUnavailableException("Problem with server");
        return JsonConvert.DeserializeObject<T>(response.Content!) 
               ?? throw new ServerUnavailableException("Problem with server");;
    } 
}