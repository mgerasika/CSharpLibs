using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HttpServer.thread
{
    public class CustomThreadManager<T>
    {
        private int MAX_THREADS = 2;
        private ThreadHandler<T> _handler;
        private List<CustomThread<T>> _threads = new List<CustomThread<T>>();
        
        public CustomThreadManager(ThreadHandler<T> handler,int threadCount)
        {
            MAX_THREADS = threadCount;
            _handler = handler;
        }

        private CustomThread<T> EnsureFreeBackgroundWorker()
        {
            CustomThread<T> res = null;

            for (int i = 0; i < MAX_THREADS; i++)
            {
                CustomThread<T> th = _threads.Count > i ? _threads[i] : null;
                if (null == th)
                {
                    th = new CustomThread<T>(_handler);
                    _threads.Add(th);
                    res = th;
                    break;
                }
                if (!th.IsBusy)
                {
                    res = th;
                    break;
                }
            }
            
            while (res == null)
            {
                Console.WriteLine("Has Busy thread.Try increase threads count for more preformance.");
                for (int i = 0; i < MAX_THREADS; i++)
                {
                    CustomThread<T> bw = _threads[i];
                    if (bw.IsBusy)
                    {
                        System.Threading.Thread.Sleep(10);
                        if (!bw.IsBusy)
                        {
                            res = bw;
                            break;
                        }
                    }
                }
            }
            Debug.Assert(null != res);
            return res;
        }

        public void Invoke(T arg)
        {
            CustomThread<T> bw = EnsureFreeBackgroundWorker();
            bw.Arg = arg;
            bw.RunWorkerAsync();
        }
    }
}