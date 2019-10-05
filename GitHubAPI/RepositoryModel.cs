using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace GitHubAPI
{
    [DataContract(Name = "repo")]
    class RepositoryModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "html_url")]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "default_branch")]
        public string DefaultBranch { get; set; }

        [DataMember(Name = "pushed_at")]
        public string PushedDateJson { get; set; }

        [IgnoreDataMember]
        public DateTime LastPushDate => DateTime.ParseExact(PushedDateJson
                    , "yyyy-MM-ddTHH:mm:ssZ"
                    , CultureInfo.InvariantCulture);

        public override string ToString() =>
            $"\n{Name}\nHtml URL: {HtmlUrl}\n{Description}"
            + $"\nDefault Branch: {DefaultBranch}\nLast Push: {LastPushDate}";
    }
}
