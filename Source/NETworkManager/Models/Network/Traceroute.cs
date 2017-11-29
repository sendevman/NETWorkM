﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NETworkManager.Models.Network
{
    public class Traceroute
    {
        #region Events
        public event EventHandler<TracerouteHopReceivedArgs> HopReceived;
        protected virtual void OnHopReceived(TracerouteHopReceivedArgs e)
        {
            HopReceived?.Invoke(this, e);
        }

        public event EventHandler TraceComplete;
        protected virtual void OnTraceComplete()
        {
            TraceComplete?.Invoke(this, System.EventArgs.Empty);
        }

        public event EventHandler<MaximumHopsReachedArgs> MaximumHopsReached;
        protected virtual void OnMaximumHopsReached(MaximumHopsReachedArgs e)
        {
            MaximumHopsReached?.Invoke(this, e);
        }

        public event EventHandler UserHasCanceled;
        protected virtual void OnUserHasCanceled()
        {
            UserHasCanceled?.Invoke(this, System.EventArgs.Empty);
        }
        #endregion

        #region Methods
        public void TraceAsync(IPAddress ipAddress, TracerouteOptions traceOptions, CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                byte[] buffer = new byte[traceOptions.Buffer];

                for (int i = 1; i < traceOptions.MaximumHops + 1; i++)
                {
                    List<Task<Tuple<PingReply, long>>> tasks = new List<Task<Tuple<PingReply, long>>>();

                    for (int y = 0; y < 3; y++)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            Stopwatch stopwatch = new Stopwatch();

                            PingReply pingReply;

                            using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
                            {
                                stopwatch.Start();

                                pingReply = ping.Send(ipAddress, traceOptions.Timeout, buffer, new System.Net.NetworkInformation.PingOptions() { Ttl = i, DontFragment = traceOptions.DontFragement });

                                stopwatch.Stop();
                            }

                            return Tuple.Create(pingReply, stopwatch.ElapsedMilliseconds);
                        }));
                    }

                    Task.WaitAll(tasks.ToArray());

                    IPAddress ipAddressHop = tasks.FirstOrDefault(x => x.Result.Item1 != null).Result.Item1.Address;

                    string hostname = string.Empty;

                    try
                    {
                        if (traceOptions.ResolveHostname && ipAddressHop != null)
                            hostname = Dns.GetHostEntry(ipAddressHop).HostName;
                    }
                    catch (SocketException) { } // Couldn't resolve hostname

                    OnHopReceived(new TracerouteHopReceivedArgs(i, tasks[0].Result.Item2, tasks[1].Result.Item2, tasks[2].Result.Item2, ipAddressHop, hostname, tasks[0].Result.Item1.Status, tasks[1].Result.Item1.Status, tasks[2].Result.Item1.Status));

                    // Check if finished
                    if (ipAddressHop != null && ipAddress.ToString() == ipAddressHop.ToString())
                    {
                        OnTraceComplete();
                        return;
                    }

                    // Check for cancel
                    if (cancellationToken.IsCancellationRequested)
                    {
                        OnUserHasCanceled();
                        return;
                    }
                }

                // Max hops reached...
                OnMaximumHopsReached(new MaximumHopsReachedArgs(traceOptions.MaximumHops));

            }, cancellationToken);
        }
        #endregion
    }
}