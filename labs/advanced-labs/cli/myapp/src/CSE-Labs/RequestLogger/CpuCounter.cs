// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace CseLabs.Middleware
{
    /// <summary>
    /// Encapsulates CPU percentage
    /// </summary>
    public class CpuCounter : IDisposable
    {
        private static readonly Process Proc = Process.GetCurrentProcess();
        private static long lastTicks = Environment.TickCount64;
        private static long lastCpu = Proc.TotalProcessorTime.Ticks;
        private static int cpu = 0;
        private static Timer timer = null;
        private static int procCount = Environment.ProcessorCount;

        /// <summary>
        /// Gets current CPU usage
        /// </summary>
        /// <returns>int</returns>
        public static int CpuPercent => cpu;

        /// <summary>
        /// Start collecting CPU metrics
        /// </summary>
        public static void Start()
        {
            if (timer != null)
            {
                Stop();
            }

            // if Windows account for hyper threading
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                procCount /= 2;
            }

            lastTicks = Environment.TickCount64;
            lastCpu = Proc.TotalProcessorTime.Ticks;

            timer = new Timer(1000);
            timer.Elapsed += TimerEvent;
            timer.Start();
        }

        /// <summary>
        /// Stop collecting CPU metrics
        /// </summary>
        public static void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
                cpu = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        // Timer worker process
        private static void TimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            // get current info
            long nowTicks = Environment.TickCount64;
            long nowCpu = Proc.TotalProcessorTime.Ticks;

            // compute CPU percentage
            cpu = (int)Math.Round((nowCpu - lastCpu) / (procCount * (nowTicks - lastTicks)) / 100.0, 0);

            // update last reading
            lastCpu = nowCpu;
            lastTicks = nowTicks;
        }
    }
}
