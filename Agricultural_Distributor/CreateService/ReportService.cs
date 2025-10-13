using Agricultural_Distributor.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.AccessControl;

namespace Agricultural_Distributor.CreateService
{
    internal class ReportService
    {
        private ReceiptDAO receiptDAO = new ReceiptDAO();
        private WarehouseInfoDAO warehouseInfoDAO = new WarehouseInfoDAO();
        private AccountDAO accountDAO = new AccountDAO();

        public void SendDailyReports()
        {
            DateTime today = DateTime.Today;
            DataTable revenue = receiptDAO.GetDailyRevenueReport(today);
            DataTable debt = receiptDAO.GetDailyDebtReportReport(today);
            DataTable expense = receiptDAO.GetDailyExpenseReport(today);
            DataTable inventory = warehouseInfoDAO.GetInventoryReport();

            string emailBody = BuildReportBody(revenue, expense, debt,inventory);
            List<string> adminEmails = accountDAO.GetAdminEmails();

            foreach (string email in adminEmails)
            {
                SendEmail(email, "BÁO CÁO HẰNG NGÀY " + today.ToString("dd/MM/yyyy"), emailBody);
            }
        }

        private string BuildReportBody(DataTable revenue, DataTable expense, DataTable debt, DataTable inventory)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family: Arial; color: #333;'>");

            sb.AppendLine($"<h2 style='color: #2e6c80; text-align: center;'>BÁO CÁO HẰNG NGÀY - {DateTime.Today:dd/MM/yyyy}</h2>");

            sb.AppendLine("<h3 style='color:#2e6c80;'>Doanh Thu</h3>");
            sb.AppendLine(GenerateHtmlTable(revenue, new[] { "RECEIPTID", "FINALAMOUNT" }, new[] { "Mã Hóa Đơn", "Tổng Tiền (VND)" }));


            sb.AppendLine("<h3 style='color:#2e6c80;'>Chi Phí</h3>");
            sb.AppendLine(GenerateHtmlTable(expense, new[] { "RECEIPTID", "FINALAMOUNT" }, new[] { "Mã Hóa Đơn", "Tổng Tiền (VND)" }));


            sb.AppendLine("<h3 style='color:#2e6c80;'>Hóa Đơn Còn Nợ</h3>");
            sb.AppendLine(GenerateHtmlTable(debt, new[] { "RECEIPTID", "FINALAMOUNT", "REPAYMENT", "REMAININGDEBT" }, new[] { "Mã Hóa Đơn", "Tổng Tiền (VND)", "Đã Trả (VND)", "Còn Nợ (VND)" }));

            sb.AppendLine("<h3 style='color:#2e6c80;'>Hàng Tồn Kho</h3>");
            sb.AppendLine(GenerateHtmlTable(inventory, new[] { "PRODUCTNAME", "QUANTITY" }, new[] { "Sản Phẩm", "Số Lượng Còn" }));

            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        private string GenerateHtmlTable(DataTable table, string[] columnKeys, string[] columnLabels)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; width: 100%; text-align: center;'>");
            sb.AppendLine("<thead style='background-color: #f2f2f2;'>");
            sb.AppendLine("<tr>");

            foreach (var label in columnLabels)
            {
                sb.AppendLine($"<th style='width:{100 / columnLabels.Length}%;'>{label}</th>");
            }

            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (DataRow row in table.Rows)
            {
                sb.AppendLine("<tr>");
                foreach (var key in columnKeys)
                {
                    sb.AppendLine($"<td>{row[key]}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }


        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("nguyenvantrong3254@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential("nguyenvantrong3254@gmail.com", "lyjq wycq ytcp dlfw");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gửi báo cáo thất bại: " + ex.Message);
            }
        }
    }
}
