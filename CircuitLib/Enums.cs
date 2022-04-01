using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public enum State
{
    Low,
    High,
    Off,
    Error,
}

public enum NodeTaskState
{
    Idle,
    Start,
    Refresh,
    Running,
}

public enum UpdateTaskState
{
    Idle,
    Waiting,
    Running,
}

public enum UpdateTaskSignal
{
    None,
    Stop,
}