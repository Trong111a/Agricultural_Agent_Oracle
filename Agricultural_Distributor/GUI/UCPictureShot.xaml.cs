using Agricultural_Distributor.DAO;
using OpenCvSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Agricultural_Distributor.GUI
{
    public partial class UCPictureShot : UserControl
    {
        private VideoCapture _capture;
        private Mat _frame;
        private WDHome wDHome;
        int emId;
        DateTime datework;

        public UCPictureShot(WDHome wDHome, int emid, DateTime dw)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            emId = emid;
            datework = dw;
            _capture = new VideoCapture(0);
            _frame = new Mat();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartCapture();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopCapture();
        }

        private void StartCapture()
        {
            Task.Run(() =>
            {
                while (_capture.IsOpened())
                {
                    _capture.Read(_frame);
                    if (!_frame.Empty())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            BitmapSource bitmapSource = BitmapSource.Create(
                            _frame.Width, _frame.Height, 96, 96,
                            System.Windows.Media.PixelFormats.Bgr24, null,
                            _frame.Data, (int)(_frame.Step() * _frame.Height), (int)_frame.Step());
                            CameraImage.Source = bitmapSource;
                        });
                    }
                }
            });
        }

        private void StopCapture()
        {
            _capture.Release();
        }

        private void btnCaptureImage_Click(object sender, RoutedEventArgs e)
        {
            if (_frame.Empty()) return;
            string projectPath = AppDomain.CurrentDomain.BaseDirectory;
            string folderPath = System.IO.Path.Combine(projectPath, "PictureSchedule");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = $"captured_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            Cv2.ImWrite(filePath, _frame);

            BitmapImage bitmapImage = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            CapturedImage.Source = bitmapImage;

            StopCapture();

            ScheduleDAO scheduleDAO = new ScheduleDAO();
            scheduleDAO.insertPicture(emId, datework, filePath);
            UCCheckWorkSchedule uCCreateOrder = new UCCheckWorkSchedule(wDHome);
            wDHome.GetUC(uCCreateOrder);
        }

    }
}
