using System.Threading;

namespace RadioTools
{
    public static class Threads
    {
        public delegate void Work(int start, int end);

        public static void StartTreadedJob(ThreadData data, Work work)
        {
            Logger.Println("Preparing threads");

            bool complete = data.numInIncompleteBatch == 0;
            int numThreads = complete ? data.completeBatches : data.completeBatches + 1;

            Thread[] threads = new Thread[numThreads];

            Logger.Println("Starting treaded work...");
            for(int i = 0; i < threads.Length; i++)
            {
                int tmp = i;
                int start = tmp * Settings.dat.maxTasksPerThread;
                int end;

                if(!complete && i == threads.Length - 1)
                    end = start + data.numInIncompleteBatch;
                else end = start + Settings.dat.maxTasksPerThread;

                threads[tmp] = new Thread(() => work(start, end));
                threads[tmp].Start();
            }

            foreach(Thread t in threads)
                t.Join();
        }

        public static ThreadData CalcReqThreads(int totalTasks)
        {
            int complete = totalTasks / Settings.dat.maxTasksPerThread;
            int incomplete = totalTasks % Settings.dat.maxTasksPerThread;

            ThreadData data = new ThreadData(totalTasks, complete, incomplete);
            
            return data;
        }

        public struct ThreadData
        {
            public int totalTasks;
            public int completeBatches;
            public int numInIncompleteBatch;

            public ThreadData(int totalTasks, int completeBatches, int numInIncompleteBatch)
            {
                this.totalTasks = totalTasks;
                this.completeBatches = completeBatches;
                this.numInIncompleteBatch = numInIncompleteBatch;
            }
        }
    }
}