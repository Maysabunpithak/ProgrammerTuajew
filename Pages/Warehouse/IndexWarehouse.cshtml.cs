using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ProgrammerTuajew.Pages.Warehouse
{
    public class IndexWarehouseModel : PageModel
    {
        public List<WarehouseInfo> InboxList = new List<WarehouseInfo>();
        public List<WarehouseInfo> SentList = new List<WarehouseInfo>();
        public string View { get; set; } = "inbox";
        public string CurrentUserEmail = "maysa@example.com"; // อีเมลผู้ใช้งานปัจจุบัน
        public bool AccessDenied { get; set; } = false; // ใช้ตรวจสอบสิทธิ์การเข้าถึง
        public string ErrorMessage { get; set; } = ""; // ข้อความแสดงข้อผิดพลาด

        public void OnGet()
        {
            if (!HasAccess("Warehouse"))
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
                        string inboxSql = "SELECT * FROM SaleD1 WHERE ReceiverDepartment = 'Warehouse'";
                        using (SqlCommand command = new SqlCommand(inboxSql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    WarehouseInfo email = new WarehouseInfo
                                    {
                                        Id = reader.GetInt32(0).ToString(),
                                        Sender = reader.GetString(1),
                                        Receiver = reader.GetString(2),
                                        ReceiverDepartment = reader.GetString(3),
                                        Subject = reader.GetString(4),
                                        Body = reader.GetString(5),
                                        DateSent = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                        IsRead = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8)
                                    };
                                    InboxList.Add(email);
                                }
                            }
                        }
                    }
                    else if (View == "sent")
                    {
                        string sentSql = "SELECT * FROM SaleD1 WHERE SenderDepartment = 'Warehouse'";
                        using (SqlCommand command = new SqlCommand(sentSql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    WarehouseInfo email = new WarehouseInfo
                                    {
                                        Id = reader.GetInt32(0).ToString(),
                                        Sender = reader.GetString(1),
                                        Receiver = reader.GetString(2),
                                        SenderDepartment = reader.GetString(3),
                                        Subject = reader.GetString(4),
                                        Body = reader.GetString(5),
                                        DateSent = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                        IsRead = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8)
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

        public JsonResult OnGetEmailDetails(string id)
        {
            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Subject, Body FROM SaleD1 WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var email = new
                                {
                                    Subject = reader.GetString(0),
                                    Body = reader.GetString(1)
                                };
                                return new JsonResult(email);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }

            return new JsonResult(new { error = "Email not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnPostMarkAsRead(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new JsonResult(new { success = false, error = "Invalid ID." });
            }

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE SaleD1 SET IsRead = 1 WHERE Id = @Id AND (IsRead = 0 OR IsRead IS NULL)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Rows affected: {rowsAffected}");


                        if (rowsAffected > 0)
                        {
                            return new JsonResult(new { success = true });
                        }
                        else
                        {
                            return new JsonResult(new { success = false, error = "Record not found or already marked as read." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        private bool HasAccess(string department)
        {
            var userDepartment = HttpContext.Session.GetString("UserDepartment");
            return userDepartment == department;
        }

        public class WarehouseInfo
        {
            public string Id { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public DateTime? DateSent { get; set; }
            public string SenderDepartment { get; set; }
            public string ReceiverDepartment { get; set; }
            public bool? IsRead { get; set; }
        }
    }
}