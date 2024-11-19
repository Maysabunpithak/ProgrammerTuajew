using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ProgrammerTuajew.Pages.SaleD1
{
    public class IndexSaleD1Model : PageModel
    {
        public List<SaleD1Info> InboxList = new List<SaleD1Info>();
        public List<SaleD1Info> SentList = new List<SaleD1Info>();
        public string View { get; set; } = "inbox";
        public string CurrentUserEmail = "maysa@example.com"; // อีเมลผู้ใช้งานปัจจุบัน
        public bool AccessDenied { get; set; } = false; // ใช้ตรวจสอบสิทธิ์การเข้าถึง
        public string ErrorMessage { get; set; } = ""; // ข้อความแสดงข้อผิดพลาด

        public void OnGet()
        {
            // ตรวจสอบสิทธิ์ของผู้ใช้
            if (!HasAccess("SaleD1"))
            {
                AccessDenied = true;
                ErrorMessage = "Your account can only be accessed by your department page.";
                Response.Redirect("/Error/AccessDenied");
                return;
            }

            View = Request.Query["view"].ToString().ToLower() ?? "inbox";

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (View == "inbox")
                    {
                        // ดึงข้อมูล Inbox
                        string inboxSql = "SELECT * FROM SaleD1 WHERE ReceiverDepartment = 'SaleD1'";
                        using (SqlCommand command = new SqlCommand(inboxSql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    SaleD1Info email = new SaleD1Info
                                    {
                                        Id = reader.GetInt32(0).ToString(),
                                        Sender = reader.GetString(1),
                                        Receiver = reader.GetString(2),
                                        ReceiverDepartment = reader.GetString(3),
                                        Subject = reader.GetString(4),
                                        Body = reader.GetString(5),
                                        DateSent = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                                    };
                                    InboxList.Add(email);
                                }
                            }
                        }
                    }
                    else if (View == "sent")
                    {
                        // ดึงข้อมูล Sent
                        string sentSql = "SELECT * FROM SaleD1 WHERE SenderDepartment = 'SaleD1'";
                        using (SqlCommand command = new SqlCommand(sentSql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    SaleD1Info email = new SaleD1Info
                                    {
                                        Id = reader.GetInt32(0).ToString(),
                                        Sender = reader.GetString(1),
                                        Receiver = reader.GetString(2),
                                        SenderDepartment = reader.GetString(3),
                                        Subject = reader.GetString(4),
                                        Body = reader.GetString(5),
                                        DateSent = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                        ReceiverDepartment = reader.GetString(3)
                                    };
                                    SentList.Add(email);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        // ฟังก์ชันช่วยตรวจสอบสิทธิ์
        private bool HasAccess(string department)
        {
            var userDepartment = HttpContext.Session.GetString("UserDepartment");
            return userDepartment == department;
        }

        public class SaleD1Info
        {
            public string Id { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public DateTime? DateSent { get; set; }
            public string SenderDepartment { get; set; }
            public string ReceiverDepartment { get; set; }
        }
    }
}
