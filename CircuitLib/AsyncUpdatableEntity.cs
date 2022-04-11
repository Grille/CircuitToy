using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CircuitLib;

public abstract class AsyncUpdatableEntity : Entity
{
    protected Task WaitTask = null;
    protected Task UpdateTask = null;
    protected UpdateTaskState UpdateState = UpdateTaskState.Idle;
    private object UpdateStateLocker = new object();
    private object UpdateInputLocker = new object();

    protected int StatsUpdatesCount;
    protected int StatsUpdatesRunCount;
    protected int StatsUpdatesQueuedCount;
    protected int StatsUpdatesDiscardCount;

    public void LockInputEnter()
    {
        Monitor.Enter(UpdateInputLocker);
    }

    public void LockInputExit()
    {
        Monitor.Exit(UpdateInputLocker);
    }

    public void Update()
    {
        StatsUpdatesCount++;
        bool runTask = false;
        bool runWait = false;
        lock (UpdateStateLocker)
        {
            if (UpdateState == UpdateTaskState.Waiting)
            {
                StatsUpdatesDiscardCount++;
                return;
            }
            if (UpdateState == UpdateTaskState.Running)
            {
                StatsUpdatesQueuedCount++;
                UpdateState = UpdateTaskState.Waiting;
                runWait = true;

            }
            if (UpdateState == UpdateTaskState.Idle)
            {
                StatsUpdatesRunCount++;
                UpdateState = UpdateTaskState.Running;
                runTask = true;
            }
        }
        if (runTask)
        {
            runUpdateTask();
        }
        if (runWait)
        {
            WaitTask = Task.Run(() => {
                UpdateTask?.Wait();
                UpdateState = UpdateTaskState.Running;
                runUpdateTask();
            });
        }
    }

    private void runUpdateTask()
    {
        UpdateTask = Task.Run(() => {
            OnUpdate();

            lock (UpdateStateLocker)
            {
                if (UpdateState == UpdateTaskState.Running)
                    UpdateState = UpdateTaskState.Idle;
            }
        });
    }

    protected abstract void OnUpdate();

    public virtual void ForceIdle()
    {
        WaitIdle();
    }

    public virtual void WaitIdle()
    {
        UpdateTask?.Wait();
    }
}
