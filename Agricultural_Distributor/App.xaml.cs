using Agricultural_Distributor.CreateService;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static System.Timers.Timer timer;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            timer = new System.Timers.Timer();
            timer.Interval = GetIntervalUntilTime();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();

            Console.WriteLine("Timer set to trigger at: " + DateTime.Now.AddMilliseconds(timer.Interval));
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ReportService service = new ReportService();
                service.SendDailyReports();
            });

            ((System.Timers.Timer)sender).Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            ((System.Timers.Timer)sender).Start();
        }

        private double GetIntervalUntilTime()
        {
            DateTime now = DateTime.Now;
            DateTime todayTime = now.Date.AddHours(0).AddMinutes(28);
            if (now > todayTime)
                todayTime = todayTime.AddDays(1); 

            return (todayTime - now).TotalMilliseconds;
        }

    }

}
