using System;
using System.Collections.Generic;
using System.Text;

namespace ObcyProto.UWP.SockJS
{
    internal class SocketAddressGenerator
    {
        private const int LowPortLimit = 7001;
        private const int HighPortLimit = 7017;

        private static readonly Random RNG = new Random();
        private static readonly List<int> PortYellowlist = new List<int>
        {
            7007,
            7009
        };

        public static int GeneratePortNumber()
        {
            var portNumber = RNG.Next(LowPortLimit, HighPortLimit);

            while (PortYellowlist.Contains(portNumber))
            {
                portNumber = RNG.Next(LowPortLimit, HighPortLimit);
            }
            return portNumber;
        }

        public static string GenerateRandomSocketNumber()
        {
            var sUid = RNG.Next();

            switch (sUid.ToString().Length)
            {
                case 1:
                    return sUid.ToString("00");
                case 2:
                    return sUid.ToString("0");
                default:
                    return sUid.ToString();
            }
        }

        public static string GenerateRandomSocketSeed(int length)
        {
            var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".ToCharArray();
            var result = new StringBuilder(length);

            for(var i = 0; i < length; i++)
            {
                result.Append(characters[RNG.Next(0, characters.Length - 1)]);
            }
            return result.ToString();
        }
    }
}
