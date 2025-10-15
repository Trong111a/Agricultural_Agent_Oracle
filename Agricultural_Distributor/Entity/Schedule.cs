using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class Schedule
    {
        private int scheduleId;
        private int employeeId;
        private string employeeName;
        private DateTime dayWork;
        private TimeSpan timeCheckIn;
        private TimeSpan timeCheckOut;
        private string linkPicture;

        public int ScheduleId { get => scheduleId; set => scheduleId = value; }
        public int EmployeeId { get => employeeId; set => employeeId = value; }
        public string EmployeeName { get => employeeName; set => employeeName = value; }
        public DateTime DayWork { get => dayWork; set => dayWork = value; }
        public TimeSpan TimeCheckIn { get => timeCheckIn; set => timeCheckIn = value; }
        public TimeSpan TimeCheckOut { get => timeCheckOut; set => timeCheckOut = value; }
        public string LinkPicture { get => linkPicture; set => linkPicture = value; }

        public Schedule(int employeeId, DateTime dayWork, TimeSpan timeCheckIn, TimeSpan timeCheckOut, string linkPicture)
        {
            EmployeeId = employeeId;
            DayWork = dayWork;
            TimeCheckIn = timeCheckIn;
            TimeCheckOut = timeCheckOut;
            LinkPicture = linkPicture;
        }

        public Schedule(int ScheduleId, int employeeId, string employeeName, DateTime dayWork, TimeSpan timeCheckIn, TimeSpan timeCheckOut, string linkPicture)
        {
            ScheduleId = ScheduleId;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            DayWork = dayWork;
            TimeCheckIn = timeCheckIn;
            TimeCheckOut = timeCheckOut;
            LinkPicture = linkPicture;
        }

    }
}
