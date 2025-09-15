namespace Softhand.Infrastructure.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Extrai o endereço do sip server de uma uri sip
    /// </summary>
    /// <param name="sipIdUri">a uri like 'sip:420@pbx.local' or 'sip:pbx.local'</param>
    /// <returns>uma string com o endereço do sip server, e.g.: pbx.local</returns>
    public static string ExtractSipServerFromSipIdUri(this string sipIdUri) => sipIdUri.Contains('@') ?  sipIdUri.Split(':')[1].Split("@")[1] : sipIdUri.Split(':')[1];
    /// <summary>
    /// Extrai o Id SIP de um URI SIP 
    /// </summary>
    /// <param name="sipIdUri">uma string sip uri do tipo 'sip:420@pbx.local'</param>
    /// <returns></returns>
    public static string ExtractSipIdFromSipIdUri(this string sipIdUri)
    {
        ArgumentNullException.ThrowIfNull(sipIdUri);

        return sipIdUri.Split(':')[1].Split("@")[0];
    }
    /// <summary>
    /// Construi uma string Sip Id Uri
    /// </summary>
    /// <param name="id">Sip Id</param>
    /// <param name="server">Sip Server</param>
    /// <returns>Uma string sip id uri do tipo 'sip:420@pbx.local'</returns>
    public static string BuildSipIdUriFromSingleId(this string id, string server) => new System.Text.StringBuilder("sip:").Append(id).Append('@').Append(server).ToString();
    public static string BuildRegistrarUriFromServer(this string server) => new System.Text.StringBuilder("sip:").Append(server).ToString();
}
