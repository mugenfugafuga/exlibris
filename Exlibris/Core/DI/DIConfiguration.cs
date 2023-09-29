using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Exlibris.Core.DI
{
    [DataContract(Name = "di_configuration")]
    public class DIConfiguration
    {
        [DataMember(Name = "singletons")]
        public List<DIItem> Singletons { get; set; } = new List<DIItem>();

        [DataMember(Name = "transients")]
        public List<DIItem> Transients { get; set; } = new List<DIItem>();
    }
}