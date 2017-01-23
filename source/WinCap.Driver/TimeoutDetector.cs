using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WinCap.Driver
{
    /// <summary>
    /// プロセスのタイムアウトを検出する機能を提供します。
    /// </summary>
    public class TimeoutDetector
    {
        /// <summary>
        /// 監視タスク
        /// </summary>
        private Task monitor;

        /// <summary>
        /// 監視状態
        /// </summary>
        private bool monitoring = true;

        /// <summary>
        /// タイムアウト時間を取得、設定します。
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// タイムアウト検出イベント
        /// </summary>
        public event EventHandler<EventArgs> Timedout;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="timeout">タイムアウト時間</param>
        public TimeoutDetector(int timeout)
        {
            this.Timeout = timeout;
            this.monitor = Task.Factory.StartNew(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (this.monitoring)
                {
                    if (this.Timeout < watch.ElapsedMilliseconds)
                    {
                        this.Timedout?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                    Thread.Sleep(10);
                }
            });
        }

        /// <summary>
        /// タイムアウト監視を完了します。
        /// </summary>
        public void Finish()
        {
            this.monitoring = false;
            this.monitor.Wait();
        }
    }
}
