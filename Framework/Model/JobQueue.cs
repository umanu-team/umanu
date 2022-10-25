/*********************************************************************
 * Umanu Framework / (C) Umanu Team / http://www.umanu.de/           *
 *                                                                   *
 * This program is free software: you can redistribute it and/or     *
 * modify it under the terms of the GNU Lesser General Public        *
 * License as published by the Free Software Foundation, either      *
 * version 3 of the License, or (at your option) any later version.  *
 *                                                                   *
 * This program is distributed in the hope that it will be useful,   *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of    *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the     *
 * GNU Lesser General Public License for more details.               *
 *                                                                   *
 * You should have received a copy of the GNU Lesser General Public  *
 * License along with this program.                                  *
 * If not, see <http://www.gnu.org/licenses/>.                       *
 *********************************************************************/

namespace Framework.Model {

    using Diagnostics;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// In-memory job queue.
    /// </summary>
    public static class JobQueue {

        /// <summary>
        /// Number of queue jobs to be executed or in execution.
        /// </summary>
        public static int Count {
            get {
                int count = JobQueue.workerQueue.Count;
                if (true == JobQueue.workerThread?.IsAlive) {
                    count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Log to use for logging.
        /// </summary>
        public static ILog Log { get; set; }

        /// <summary>
        /// Delegate  to be executed on a regular basis for creation
        /// of new timer job.
        /// </summary>
        public static Action TimerJob {
            get {
                return JobQueue.timerJob;
            }
            set {
                JobQueue.timerJob = value;
                if (null != value) {
                    if (UtcDateTime.MaxValue == JobQueue.TimerJobStartTime) {
                        int secondsToWait = new Random().Next(0, (int)JobQueue.TimerJobWaitingTime.TotalSeconds);
                        JobQueue.TimerJobStartTime = UtcDateTime.Now.AddSeconds(secondsToWait);
                    }
                    JobQueue.StartWorkerThread();
                }
            }
        }
        private static Action timerJob;

        /// <summary>
        /// Date and time of next execution of timer job.
        /// </summary>
        public static DateTime TimerJobStartTime { get; private set; } = UtcDateTime.MaxValue;

        /// <summary>
        /// Waiting time between executions of timer job.
        /// </summary>
        public static TimeSpan TimerJobWaitingTime { get; set; } = new TimeSpan(6, 0, 0);

        /// <summary>
        /// Waiting thread for restarting worker thread.
        /// </summary>
        private static Thread waiterThread;

        /// <summary>
        /// Lock for waiter thread.
        /// </summary>
        private static readonly object waiterThreadLock = new object();

        /// <summary>
        /// Queue for worker thread.
        /// </summary>
        private static readonly ConcurrentQueue<Action> workerQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// Worker thread for sending emails.
        /// </summary>
        private static Thread workerThread;

        /// <summary>
        /// Lock for worker thread.
        /// </summary>
        private static readonly object workerThreadLock = new object();

        /// <summary>
        /// Aborts all running jobs by raising a ThreadAbortException.
        /// </summary>
        public static void Abort() {
            lock (JobQueue.waiterThreadLock) {
                if (true == JobQueue.waiterThread?.IsAlive) {
                    JobQueue.waiterThread?.Abort();
                }
            }
            lock (JobQueue.workerThreadLock) {
                if (true == JobQueue.workerThread?.IsAlive) {
                    JobQueue.workerThread?.Abort();
                }
            }
            return;
        }

        /// <summary>
        /// Adds a queue job to the end of the job queue and starts
        /// the execution of all queued jobs.
        /// </summary>
        /// <param name="queueJob">queue job to add to end of job
        /// queue</param>
        public static void Enqueue(Action queueJob) {
            JobQueue.workerQueue.Enqueue(queueJob);
            JobQueue.StartWorkerThread();
            return;
        }

        /// <summary>
        /// Adds a queue job to the beginning of the job queue and
        /// starts the execution of all queued jobs.
        /// </summary>
        /// <param name="queueJob">queue job to add to beginning of
        /// job queue</param>
        internal static void EnqueueAtBeginning(Action queueJob) {
            var queuedJobs = new List<Action>(JobQueue.workerQueue.Count);
            while (JobQueue.workerQueue.TryDequeue(out Action queuedJob)) {
                queuedJobs.Add(queuedJob);
            }
            JobQueue.workerQueue.Enqueue(queueJob);
            foreach (var queuedJob in queuedJobs) {
                JobQueue.workerQueue.Enqueue(queuedJob);
            }
            JobQueue.StartWorkerThread();
            return;
        }

        /// <summary>
        /// Executes all jobs from queue.
        /// </summary>
        private static void ExecuteJobs() {
            var failedJobs = new List<Action>();
            while (JobQueue.workerQueue.TryDequeue(out Action queueJob)) {
                if (null != queueJob) {
                    try {
                        JobQueue.Log?.WriteEntry("Executing queued job " + queueJob.Target.ToString() + "...", LogLevel.Debug);
                        queueJob.Invoke();
                        JobQueue.Log?.WriteEntry("...finished execution of queued job " + queueJob.Target.ToString() + " successfully.", LogLevel.Debug);
                    } catch (Exception exception) {
                        failedJobs.Add(queueJob);
                        JobQueue.Log?.WriteEntry(exception, LogLevel.Error);
                        JobQueue.Log?.WriteEntry("...execution of queued job " + queueJob.Target.ToString() + " failed.", LogLevel.Debug);
#if DEBUG
                        throw;
#endif
                    }
                }
            }
            foreach (var failedJob in failedJobs) {
                JobQueue.workerQueue.Enqueue(failedJob);
            }
            if (null != JobQueue.TimerJob && UtcDateTime.Now >= JobQueue.TimerJobStartTime) {
                try {
                    JobQueue.TimerJob();
                    JobQueue.TimerJobStartTime = UtcDateTime.Now.Add(JobQueue.TimerJobWaitingTime);
                } catch (Exception exception) {
                    JobQueue.Log?.WriteEntry(exception, LogLevel.Error);
#if DEBUG
                    throw;
#endif
                }
            }
            if (null != JobQueue.TimerJob || !JobQueue.workerQueue.IsEmpty) {
                JobQueue.StartWaiterThread();
            }
            return;
        }

        /// <summary>
        /// Starts a new waiter thread if no waiter thread is active.
        /// </summary>
        private static void StartWaiterThread() {
            lock (JobQueue.waiterThreadLock) {
                if (true != JobQueue.waiterThread?.IsAlive) {
                    JobQueue.waiterThread = new Thread(JobQueue.WaitForNextExecution) {
                        IsBackground = true,
                        Priority = ThreadPriority.BelowNormal
                    };
                    JobQueue.waiterThread.Start();
                }
            }
            return;
        }

        /// <summary>
        /// Starts a new worker thread if no worker thread is active.
        /// </summary>
        private static void StartWorkerThread() {
            lock (JobQueue.workerThreadLock) {
                if (true != JobQueue.workerThread?.IsAlive) {
                    JobQueue.workerThread = new Thread(JobQueue.ExecuteJobs) {
                        IsBackground = false,
                        Priority = ThreadPriority.BelowNormal
                    };
                    JobQueue.workerThread.Start();
                }
            }
            return;
        }

        /// <summary>
        /// Waits for a while, executes jobs afterwards.
        /// </summary>
        private static void WaitForNextExecution() {
            var delay = new TimeSpan(0, 30, 0);
            var timeSpanUntilNextScheduledJob = JobQueue.TimerJobStartTime - UtcDateTime.Now;
            if (delay > timeSpanUntilNextScheduledJob) {
                if (timeSpanUntilNextScheduledJob > TimeSpan.Zero) {
                    delay = timeSpanUntilNextScheduledJob;
                } else {
                    delay = TimeSpan.Zero;
                }
            }
            Thread.Sleep(delay);
            JobQueue.StartWorkerThread();
            return;
        }

    }

}