using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace network_wpf
{
    public class ClientRequest
    {
        public String Command { get; set; }
        public ChatMessage Message { get; set; }
    }
}
