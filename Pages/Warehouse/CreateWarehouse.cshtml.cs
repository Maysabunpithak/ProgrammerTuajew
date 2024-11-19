using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace ProgrammerTuajew.Pages.Warehouse
{
    public class CreateWarehouseModel : PageModel
    {
        public WarehouseInfo WarehouseInfo = new WarehouseInfo();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
            // ใช้สำหรับแสดงฟอร์ม
        }

        public void OnPost()
        {
            // รับค่าจากฟอร์ม
            WarehouseInfo.Sender = Request.Form["Sender"];
            WarehouseInfo.Receiver = Request.Form["Receiver"];
            WarehouseInfo.Subject = Request.Form["Subject"];
            WarehouseInfo.Body = Request.Form["Body"];
            WarehouseInfo.DateSent = DateTime.TryParse(Request.Form["DateSent"], out DateTime date) ? date : (DateTime?)null;
            WarehouseInfo.SenderDepartment = "Warehouse"; // ระบุแผนกที่ส่ง
            WarehouseInfo.ReceiverDepartment = Request.Form["ReceiverDepartment"]; // รับแผนกที่ต้องการส่ง

            // ตรวจสอบค่าที่จำเป็น
            if (string.IsNullOrEmpty(WarehouseInfo.Sender) || string.IsNullOrEmpty(WarehouseInfo.Receiver) || string.IsNullOrEmpty(WarehouseInfo.Subject))
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
                        command.Parameters.AddWithValue("@Sender", WarehouseInfo.Sender);
                        command.Parameters.AddWithValue("@Receiver", WarehouseInfo.Receiver);
                        command.Parameters.AddWithValue("@Subject", WarehouseInfo.Subject);
                        command.Parameters.AddWithValue("@Body", WarehouseInfo.Body);
                        command.Parameters.AddWithValue("@DateSent", WarehouseInfo.DateSent.HasValue ? (object)WarehouseInfo.DateSent.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@SenderDepartment", WarehouseInfo.SenderDepartment);
                        command.Parameters.AddWithValue("@ReceiverDepartment", WarehouseInfo.ReceiverDepartment);

                        command.ExecuteNonQuery();
                    }
                }
                SuccessMessage = "เพิ่มข้อมูลสำเร็จ";
                Response.Redirect("/Warehouse/IndexWarehouse?view=sent");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"เกิดข้อผิดพลาด: {ex.Message}";
            }
        }
    }

    public class WarehouseInfo
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
