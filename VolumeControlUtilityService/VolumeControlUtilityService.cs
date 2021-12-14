//.net framework service
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;

//using System.Timers;
//using System.Runtime.InteropServices;

// .NET 5 Service
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

// Project dependencies
using VolumeControlUtility;

namespace VolumeControlUtilityService
{

    public class VolumeControlUtilityService2 : BackgroundService
    {
        private readonly ILogger<VolumeControlUtilityService2> _logger;

        public VolumeControlUtilityService2(ILogger<VolumeControlUtilityService2> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service started");

            await Task.Factory.StartNew(() => VolumeControlUtility.Program.ServiceMain(_logger), TaskCreationOptions.LongRunning);

            _logger.LogInformation("Service stopped");
        }
    }


    //public enum ServiceState
    //{
    //    SERVICE_STOPPED = 0x00000001,
    //    SERVICE_START_PENDING = 0x00000002,
    //    SERVICE_STOP_PENDING = 0x00000003,
    //    SERVICE_RUNNING = 0x00000004,
    //    SERVICE_CONTINUE_PENDING = 0x00000005,
    //    SERVICE_PAUSE_PENDING = 0x00000006,
    //    SERVICE_PAUSED = 0x00000007,
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct ServiceStatus
    //{
    //    public int dwServiceType;
    //    public ServiceState dwCurrentState;
    //    public int dwControlsAccepted;
    //    public int dwWin32ExitCode;
    //    public int dwServiceSpecificExitCode;
    //    public int dwCheckPoint;
    //    public int dwWaitHint;
    //};
    //public partial class VolumeControlUtilityService : ServiceBase
    //{
    //    private const String EVENT_LOG_SOURCE = "VCU_Source";
    //    private const String EVENT_LOG_NAME = "VCU_Log";

    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

    //    private int eventId = 1;

    //    private Task serverTask;

    //    public VolumeControlUtilityService()
    //    {
    //        InitializeComponent();
    //        eventLog = new EventLog();
    //        if (!EventLog.SourceExists(EVENT_LOG_SOURCE))
    //        {
    //            EventLog.CreateEventSource(
    //                EVENT_LOG_SOURCE, EVENT_LOG_NAME);
    //        }
    //        eventLog.Source = EVENT_LOG_SOURCE;
    //        eventLog.Log = EVENT_LOG_NAME;
    //    }

    //    protected override void OnStart(string[] args)
    //    {
    //        eventLog.WriteEntry("In OnStart.");

    //        // Update the service state to Start Pending.
    //        ServiceStatus serviceStatus = new ServiceStatus();
    //        serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
    //        serviceStatus.dwWaitHint = 100000;
    //        SetServiceStatus(this.ServiceHandle, ref serviceStatus);

    //        // setup timer
    //        Timer timer = new Timer();
    //        timer.Interval = 60000; // 60 seconds
    //        timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
    //        timer.Start();

    //        ////////////////////////////////////////////
    //        /// VCU

    //        this.serverTask = Task.Factory.StartNew(() => VolumeControlUtility.Program.ServiceMain(eventLog), TaskCreationOptions.LongRunning);


    //        ///
    //        ////////////////////////////////////////////

    //        // Update the service state to Running.
    //        serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
    //        SetServiceStatus(this.ServiceHandle, ref serviceStatus);
    //    }

    //    public void OnTimer(object sender, ElapsedEventArgs args)
    //    {
    //        // TODO: Insert monitoring activities here.
    //        eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
    //    }

    //    protected override void OnPause()
    //    {
    //        eventLog.WriteEntry("In OnPause.");
    //    }

    //    protected override void OnContinue()
    //    {
    //        eventLog.WriteEntry("In OnContinue.");
    //    }

    //    protected override void OnStop()
    //    {
    //        eventLog.WriteEntry("In OnStop.");

    //        // Update the service state to Stop Pending.
    //        ServiceStatus serviceStatus = new ServiceStatus();
    //        serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
    //        serviceStatus.dwWaitHint = 100000;
    //        SetServiceStatus(this.ServiceHandle, ref serviceStatus);

    //        // Update the service state to Stopped.
    //        serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
    //        SetServiceStatus(this.ServiceHandle, ref serviceStatus);
    //    }
    //}
}
