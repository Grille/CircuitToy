using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib;

public abstract class AsyncUpdatableEntity : Entity
{
    protected Task WaitTask = null;
    protected Task UpdateTask = null;
    protected UpdateTaskState UpdateState = UpdateTaskState.Idle;

    protected int StatsUpdatesCount;
    protected int StatsUpdatesRunCount;
    protected int StatsUpdatesQueuedCount;
    protected int StatsUpdatesDiscardCount;


    public void Update()
    {
        lock (this)
        {
            StatsUpdatesCount++;
            if (UpdateState == UpdateTaskState.Waiting)
            {
                StatsUpdatesDiscardCount++;
                return;
            }
            if (UpdateState == UpdateTaskState.Running)
            {
                StatsUpdatesQueuedCount++;
                UpdateState = UpdateTaskState.Waiting;
                WaitTask = Task.Run(() => {
                    UpdateTask?.Wait();
                    UpdateState = UpdateTaskState.Running;
                    runUpdateTask();
                });
            }
            if (UpdateState == UpdateTaskState.Idle)
            {
                StatsUpdatesRunCount++;
                UpdateState = UpdateTaskState.Running;
                runUpdateTask();
            }
        }
    }

    private void runUpdateTask()
    {
        UpdateTask = Task.Run(() => {
            OnUpdate();

            lock (this)
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
