using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public class QueueTaskManager
    {
        Queue<System.Action> queueTask;
        bool isBusy = false;

        public QueueTaskManager()
        {
            queueTask = new Queue<System.Action>();
        }

        public void PushTask(System.Action task)
        {
            queueTask.Enqueue(task);

            if (isBusy == false)
                ExecuteNextTask();
        }

        public void ExecuteNextTask()
        {
            isBusy = false;

            if (queueTask.Count > 0)
            {
                isBusy = true;
                var task = queueTask.Dequeue();

                if (task != null)
                    task();
            }
        }
    }
}
