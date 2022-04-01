using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VIETLOTT_SyncData
{
    public partial class Service1 : ServiceBase
    {
        private static Timer _timer;
        const double TIMEOUT = 3600000;

        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            if (Globals.ConnectSapB1() == true)
            {
                //Globals.GetPos("14100001");
                //Globals.GetLocation();
                //Globals.GetBranch();

                //Globals.GetAgency();


                //Globals.BusinessPartnerAndProject();
                Globals.GetAgencyByDate();
                Globals.GetPosByDate();
            }
            
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _timer = new Timer(); //This will set the default interval
            _timer.AutoReset = false;
            _timer.Elapsed += OnTimer;
            _timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                if (Globals.ConnectSapB1() == true)
                {
                    //Globals.GetAgency();
                    //Globals.BusinessPartnerAndProject();
                    Globals.GetAgencyByDate();
                    Globals.GetPosByDate();
                }
            }
            catch(Exception e)
            {
                Globals.WriteLog(e.ToString());
            }
            _timer.Stop();
            _timer.Interval = TIMEOUT; //Service time
            _timer.Start();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }
    }
}
