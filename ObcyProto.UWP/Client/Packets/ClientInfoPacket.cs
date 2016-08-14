using Newtonsoft.Json.Linq;
using ObcyProto.UWP.Client.Identity;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class ClientInfoPacket : Packet
    {
        public ClientInfoPacket(bool isMobile, UserAgent userAgent, string hash, int ckey, bool recevSent)
        {
            Header = "_cinfo";

            var testData = new JObject
            {
                ["ckey"] = ckey,
                ["recevSent"] = recevSent.ToString().ToLower()
            };

            Data = new JObject
            {
                ["mobile"] = isMobile,
                ["cver"] = userAgent.ToString(),
                ["adf"] = "ajaxPHP", // FIXME: Currently hard-coded, new protocol analysis yielded just this result.
                ["hash"] = hash,
                ["testdata"] = testData
            };

        }
    }
}
