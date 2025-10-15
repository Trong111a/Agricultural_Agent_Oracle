using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class Employee
    {
        private int employeeId;
        private string employeeName;
        private DateTime birthday;
        private string sex;
        private string employeeAddress;
        private string phoneNumber;
        private string email;
        private int isActive;
        private int position;

        public int EmployeeId { get => employeeId; set => employeeId = value; }
        public string EmployeeName { get => employeeName; set => employeeName = value; }
        public DateTime Birthday { get => birthday; set => birthday = value; }
        public string Sex { get => sex; set => sex = value; }
        public string EmployeeAddress { get => employeeAddress; set => employeeAddress = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string Email { get => email; set => email = value; }

        public int IsActive { get => isActive; set => isActive = value; }

        public string Status => IsActive == 1 ? "Đang làm" : "Đã nghỉ";

        public bool IsSelected { get; set; }
        public int Position { get => position; set => position = value; }

        public Employee() { }

        public Employee(int employeeId, string employeeName, DateTime birthday, string sex, string employeeAddress, string phoneNumber, string email)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Birthday = birthday;
            Sex = sex;
            EmployeeAddress = employeeAddress;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public Employee(int employeeId, string employeeName, DateTime birthday, string sex, string employeeAddress, string phoneNumber, string email, int position)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Birthday = birthday;
            Sex = sex;
            EmployeeAddress = employeeAddress;
            PhoneNumber = phoneNumber;
            Email = email;
            Position = position;
        }

    }
}
