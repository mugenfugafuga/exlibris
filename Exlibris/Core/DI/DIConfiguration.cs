using System.Collections.Generic;

namespace Exlibris.Core.DI
{
    public class DIConfiguration
    {
        public List<DIItem> Singletons { get; set; } = new List<DIItem>();

        public List<DIItem> Transients { get; set; } = new List<DIItem>();
    }
}