using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProtocolException : Exception
{
    public ProtocolException() : base()
    {
    }

    public ProtocolException(string message) : base(message)
    {
    }
}
