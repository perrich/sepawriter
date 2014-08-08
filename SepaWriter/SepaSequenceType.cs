using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perrich.SepaWriter
{
    public enum SepaSequenceType
    {
        // One-Off
        OOFF,
        // First recurring payment
        FIRST,
        // A recurring payment (which is not the first or the last)
        RCUR,
        // Last recurring payment
        FINAL
    }
}
