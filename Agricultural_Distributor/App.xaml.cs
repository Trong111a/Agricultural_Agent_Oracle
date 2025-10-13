using Agricultural_Distributor.CreateService;
using System; 
using System.Windows;
using System.Windows.Threading; 

namespace Agricultural_Distributor
{
    public partial class App : Application
    {
        private static DispatcherTimer dispatcherTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(GetIntervalUntilTime());

            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

            Console.WriteLine("Timer set to trigger at: " + DateTime.Now.Add(dispatcherTimer.Interval));
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ReportService service = new ReportService();
            service.SendDailyReports();

            Console.WriteLine($"Report sent at: {DateTime.Now}");
            ((DispatcherTimer)sender).Interval = TimeSpan.FromDays(1);

        }

        private double GetIntervalUntilTime()
        {
            DateTime now = DateTime.Now;
            DateTime todayTime = now.Date.AddHours(17).AddMinutes(7);

            if (now > todayTime)
                todayTime = todayTime.AddDays(1);

            return (todayTime - now).TotalMilliseconds;
        }
    }
}