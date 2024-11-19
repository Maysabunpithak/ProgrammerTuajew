using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace ProgrammerTuajew.Pages.SaleD2
{
    public class CreateSaleD2Model : PageModel
    {
        public SaleD2Info SaleInfo = new SaleD2Info();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
            // ใช้สำหรับแสดงฟอร์ม
        }

        public void OnPost()
        {
            // รับค่าจากฟอร์ม
            SaleInfo.Sender = Request.Form["Sender"];
            SaleInfo.Receiver = Request.Form["Receiver"];
            SaleInfo.Subject = Request.Form["Subject"];
            SaleInfo.Body = Request.Form["Body"];
            SaleInfo.DateSent = DateTime.TryParse(Request.Form["DateSent"], out DateTime date) ? date : (DateTime?)null;
            SaleInfo.SenderDepartment = "SaleD2"; // ระบุแผนกที่ส่ง
            SaleInfo.ReceiverDepartment = Request.Form["ReceiverDepartment"]; // รับแผนกที่ต้องการส่ง

            // ตรวจสอบค่าที่จำเป็น
            if (string.IsNullOrEmpty(SaleInfo.Sender) || string.IsNullOrEmpty(SaleInfo.Receiver) || string.IsNullOrEmpty(SaleInfo.Subject))
            {
                ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน";
                return;
            }

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO SaleD1 (Sender, Receiver, Subject, Body, DateSent, SenderDepartment, ReceiverDepartment) " +
                                 "VALUES (@Sender, @Receiver, @Subject, @Body, @DateSent, @SenderDepartment, @ReceiverDepartment)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Sender", SaleInfo.Sender);
                        command.Parameters.AddWithValue("@Receiver", SaleInfo.Receiver);
                        command.Parameters.AddWithValue("@Subject", SaleInfo.Subject);
                        command.Parameters.AddWithValue("@Body", SaleInfo.Body);
                        command.Parameters.AddWithValue("@DateSent", SaleInfo.DateSent.HasValue ? (object)SaleInfo.DateSent.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@SenderDepartment", SaleInfo.SenderDepartment);
                        command.Parameters.AddWithValue("@ReceiverDepartment", SaleInfo.ReceiverDepartment);

                        command.ExecuteNonQuery();
                    }
                }
                SuccessMessage = "เพิ่มข้อมูลสำเร็จ";
                Response.Redirect("/SaleD2/IndexSaleD2?view=sent");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"เกิดข้อผิดพลาด: {ex.Message}";
            }
        }
    }

    public class SaleD2Info
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? DateSent { get; set; }
        public string SenderDepartment { get; set; }
        public string ReceiverDepartment { get; set; }
    }
}
