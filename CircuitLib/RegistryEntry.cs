using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public abstract class RegistryEntry
{
    public string Name { get; set; }
    public Type Type;

    public abstract void Create(Circuit target);
}
