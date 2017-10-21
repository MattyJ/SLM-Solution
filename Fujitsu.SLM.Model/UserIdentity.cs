using System;

namespace Fujitsu.SLM.Model
{
    [Serializable]
    public class UserIdentity
    {
        public string SourceIpAddress { get; set; }
        public string NameIdentifier { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
    }
}
