using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quisitive.Framework.Repository
{
    public class GetSitemapRequest
    {
        public string StyleSheet { get; set; }
        public string Protocol { get; set; }
        public string Domain { get; set; }
        public string ContentRoot { get; set; }
    }
}
