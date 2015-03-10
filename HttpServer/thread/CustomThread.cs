using System.ComponentModel;
using System.Diagnostics;

namespace HttpServer.thread
{
    public class CustomThread<T>
    {
        private BackgroundWorker _bw;
        private ThreadHandler<T> _handler;

        public CustomThread(ThreadHandler<T> handler)
        {
            _handler = handler;

            _bw = new BackgroundWorker();
            _bw.DoWork += bw_DoWork;
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = false;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            lock (bw)
            {
                if (this.Arg != null)
                {
                    _handler.Invoke(this.Arg);
                }
            }
        }

        public bool IsBusy
        {
            get { return _bw.IsBusy; }
        }

        public T Arg { get; set; }

        internal void RunWorkerAsync()
        {
            lock (_bw)
            {
                Debug.Assert(!_bw.IsBusy);
                _bw.RunWorkerAsync();
            }
        }
    }
}