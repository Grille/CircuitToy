using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib_Tests;

public class FailException : Exception
{
    public FailException(string message) : base(message) { }
}

public class SuccsessException : Exception
{
    public SuccsessException(string message) : base(message) { }
}
