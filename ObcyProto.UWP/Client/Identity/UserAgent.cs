namespace ObcyProto.UWP.Client.Identity
{
    public class UserAgent
    {
        public string Name { get; }
        public string Version { get; }

        public UserAgent(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public override string ToString()
        {
            return $"{Name}{Version}";
        }
    }
}
