using Agricultural_Distributor.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Agricultural_Distributor.DAO
{
    internal class ScheduleDAO
    {
        ConnectOracle connectOracle = new();
        Schedule schedule;

        public ScheduleDAO() { }

        public ScheduleDAO(Employee employee)
        {
            this.schedule = schedule;
        }

        public List<Schedule> LoadSchedule(DateTime date)
        {
            List<Schedule> schedules = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.Connection = connectOracle.oraCon; 
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText = @"
                SELECT 
                    S.SCHEDULEID, 
                    E.EMPLOYEEID, 
                    E.EMPLOYEENAME, 
                    S.DATEWORK, 
                    S.TIMECHECKIN, 
                    S.TIMECHECKOUT, 
                    S.LINKPICTURE 
                FROM SCHEDULE S
                INNER JOIN EMPLOYEE E ON S.EMPLOYEEID = E.EMPLOYEEID
                WHERE TRUNC(S.DATEWORK) = TRUNC(:p_date)";

            oraCmd.Parameters.Add("p_date", OracleDbType.Date).Value = date.Date;

            OracleDataReader reader = oraCmd.ExecuteReader(); 

            while (reader.Read())
            {
                int ScheduleId = reader.GetInt32(0);
                int EmployeeId = reader.GetInt32(1);
                string EmployeeName = reader.GetString(2);
                DateTime DateWork = reader.GetDateTime(3);

                OracleIntervalDS CheckInInterval = (OracleIntervalDS)reader.GetOracleValue(4);
                OracleIntervalDS CheckOutInterval = (OracleIntervalDS)reader.GetOracleValue(5);

                TimeSpan TimeCheckIn = new TimeSpan(
                    CheckInInterval.Days * 24 + CheckInInterval.Hours,
                    CheckInInterval.Minutes,
                    CheckInInterval.Seconds);

                TimeSpan TimeCheckOut = new TimeSpan(
                    CheckOutInterval.Days * 24 + CheckOutInterval.Hours,
                    CheckOutInterval.Minutes,
                    CheckOutInterval.Seconds);


                string LinkPicture = reader.IsDBNull(6) ? null : reader.GetString(6);

                Schedule schedule = new(ScheduleId, EmployeeId, EmployeeName, DateWork, TimeCheckIn, TimeCheckOut, LinkPicture);
                schedules.Add(schedule);
            }
            reader.Close();
            connectOracle.Close();
            return schedules;
        }

        public void InsertSchedule(Schedule schedule)
        {
            try
            {
                connectOracle.Connect();

                DateTime today = DateTime.Today;

                using (OracleCommand checkCmd = new(@"
            SELECT COUNT(*) FROM Schedule 
            WHERE employeeid = :employeeid AND TRUNC(datework) = TRUNC(:datework)", connectOracle.oraCon))
                {
                    checkCmd.Parameters.Add(":employeeid", OracleDbType.Int32).Value = schedule.EmployeeId;
                    checkCmd.Parameters.Add(":datework", OracleDbType.Date).Value = today;

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Nhân viên đã được thêm vào lịch làm việc hôm nay.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                using (OracleCommand cmd = new(@"
                    INSERT INTO schedule (employeeid, datework, timecheckin, timecheckout, linkpicture)
                    VALUES (:employeeid, :datework, :timecheckin, :timecheckout, :linkpicture)", connectOracle.oraCon))
                {
                    cmd.Parameters.Add(":employeeid", OracleDbType.Varchar2).Value = schedule.EmployeeId;
                    //cmd.Parameters.Add(":employeename", OracleDbType.Varchar2).Value = schedule.EmployeeName;
                    cmd.Parameters.Add(":datework", OracleDbType.Date).Value = today;

                    cmd.Parameters.Add(":timecheckin", OracleDbType.IntervalDS).Value = schedule.TimeCheckIn;
                    cmd.Parameters.Add(":timecheckout", OracleDbType.IntervalDS).Value = schedule.TimeCheckOut;

                    cmd.Parameters.Add(":linkpicture", OracleDbType.Varchar2).Value = "";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm lịch làm việc: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connectOracle.Close();
            }
        }


        public void updateSchedule(int employeeId, DateTime dateWork, TimeSpan timeCheckIn, TimeSpan timeCheckOut)
        {
            connectOracle.Connect();

            string query = @" UPDATE Schedule SET timecheckin = TO_DSINTERVAL(:timecheckin), timecheckout = TO_DSINTERVAL(:timecheckout) 
                    WHERE employeeid = :employeeid AND TRUNC(datework) = TRUNC(:datework)";


            using (OracleCommand cmd = new(query, connectOracle.oraCon))
            {
                //cmd.Parameters.Add(":timecheckin", OracleDbType.Varchar2).Value = timeCheckIn.ToString();
                //cmd.Parameters.Add(":timecheckout", OracleDbType.Varchar2).Value = timeCheckOut.ToString();
                cmd.Parameters.Add("timecheckin", OracleDbType.Varchar2).Value = ToIntervalString(timeCheckIn);
                cmd.Parameters.Add("timecheckout", OracleDbType.Varchar2).Value = ToIntervalString(timeCheckOut);

                cmd.Parameters.Add(":employeeid", OracleDbType.Int32).Value = employeeId;
                cmd.Parameters.Add(":datework", OracleDbType.Date).Value = dateWork;

                cmd.ExecuteNonQuery();
            }
            connectOracle.Close();
        }
        string ToIntervalString(TimeSpan ts)
        {
            return string.Format("+{0} {1:D2}:{2:D2}:{3:D2}",
                                 (int)ts.TotalDays, ts.Hours, ts.Minutes, ts.Seconds);
        }

        public void insertPicture(int employeeId, DateTime dateWork, string imagePath)
        {
            connectOracle.Connect();

            string query = @"UPDATE Schedule 
                    SET linkpicture = :linkpicture 
                    WHERE employeeid = :employeeid AND TRUNC(datework) = TRUNC(:datework)";

            using (OracleCommand cmd = new(query, connectOracle.oraCon))
            {
                cmd.Parameters.Add(":linkpicture", OracleDbType.Varchar2).Value = imagePath;
                cmd.Parameters.Add(":employeeid", OracleDbType.Int32).Value = employeeId;
                cmd.Parameters.Add(":datework", OracleDbType.Date).Value = dateWork;

                cmd.ExecuteNonQuery();
            }
            connectOracle.Close();
        }
    }
}