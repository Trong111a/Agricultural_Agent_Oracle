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
                DateTime CheckInDateTime = reader.GetDateTime(4);
                DateTime CheckOutDateTime = reader.GetDateTime(5);

                TimeSpan TimeCheckIn = CheckInDateTime.TimeOfDay;
                TimeSpan TimeCheckOut = CheckOutDateTime.TimeOfDay;

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
                    checkCmd.Parameters.Add(":employeeid", OracleDbType.Varchar2).Value = schedule.EmployeeId;
                    checkCmd.Parameters.Add(":datework", OracleDbType.Date).Value = today;

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Nhân viên đã được thêm vào lịch làm việc hôm nay.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                using (OracleCommand cmd = new(@"
            INSERT INTO schedule (employeeid, employeename, datework, timecheckin, timecheckout, linkpicture)
            VALUES (:employeeid, :employeename, :datework, :timecheckin, :timecheckout, :linkpicture)", connectOracle.oraCon))
                {
                    cmd.Parameters.Add(":employeeid", OracleDbType.Varchar2).Value = schedule.EmployeeId;
                    cmd.Parameters.Add(":employeename", OracleDbType.Varchar2).Value = schedule.EmployeeName;
                    cmd.Parameters.Add(":datework", OracleDbType.Date).Value = today;

                    cmd.Parameters.Add(":timecheckin", OracleDbType.Varchar2).Value = schedule.TimeCheckIn.ToString();
                    cmd.Parameters.Add(":timecheckout", OracleDbType.Varchar2).Value = schedule.TimeCheckOut.ToString();

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

            string query = @"UPDATE Schedule 
                    SET timecheckin = :timecheckin, timecheckout = :timecheckout 
                    WHERE employeeid = :employeeid AND TRUNC(datework) = TRUNC(:datework)";

            using (OracleCommand cmd = new(query, connectOracle.oraCon))
            {
                cmd.Parameters.Add(":timecheckin", OracleDbType.Varchar2).Value = timeCheckIn.ToString();
                cmd.Parameters.Add(":timecheckout", OracleDbType.Varchar2).Value = timeCheckOut.ToString();

                cmd.Parameters.Add(":employeeid", OracleDbType.Int32).Value = employeeId;
                cmd.Parameters.Add(":datework", OracleDbType.Date).Value = dateWork;

                cmd.ExecuteNonQuery();
            }
            connectOracle.Close();
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