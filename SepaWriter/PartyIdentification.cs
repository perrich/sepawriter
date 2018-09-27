using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perrich.SepaWriter
{
    public class PartyIdentification
    {
        public string Name { get; set; }
        public PostalAddress PostalAddress { get; set; }
        public Party Id { get; set; }
    }
}
