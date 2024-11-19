using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace ProgrammerTuajew.Pages.SaleD3
{
    public class EditSaleD3Model : PageModel
    {
        public SaleD3Info SaleInfo = new SaleD3Info();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
            string itemId = Request.Query["itemid"];
            if (string.IsNullOrEmpty(itemId))
            {
                ErrorMessage = "Invalid ID";
                return;
            }

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Id, Sender, Receiver, ReceiverDepartment, Subject, Body, DateSent FROM SaleD1 WHERE Id = @Id AND SenderDepartment = 'SaleD3'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", itemId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SaleInfo.Id = reader.GetInt32(0).ToString();
                                SaleInfo.Sender = reader.GetString(1);
                                SaleInfo.Receiver = reader.GetString(2);
                                SaleInfo.ReceiverDepartment = reader.IsDBNull(3) ? null : reader.GetString(3);
                                SaleInfo.Subject = reader.GetString(4);
                                SaleInfo.Body = reader.GetString(5);
                                SaleInfo.DateSent = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            }
                            else
                            {
                                ErrorMessage = "No record found with the given ID for SaleD3.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error retrieving data: {ex.Message}";
            }
        }

        public void OnPost()
        {
            SaleInfo.Id = Request.Form["Id"];
            SaleInfo.Subject = Request.Form["Subject"];
            SaleInfo.Body = Request.Form["Body"];
            SaleInfo.DateSent = DateTime.TryParse(Request.Form["DateSent"], out DateTime date) ? date : (DateTime?)null;

            string viewType = Request.Query["view"].ToString().ToLower(); // อ่านค่า view จาก Query String

            if (string.IsNullOrEmpty(SaleInfo.Id))
            {
                ErrorMessage = "Invalid ID.";
                return;
            }

            if (string.IsNullOrEmpty(SaleInfo.Subject) || string.IsNullOrEmpty(SaleInfo.Body))
            {
                ErrorMessage = "Please fill in all required fields.";
                return;
            }

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE SaleD1 SET Subject = @Subject, Body = @Body, DateSent = @DateSent WHERE Id = @Id AND SenderDepartment = 'SaleD3'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", SaleInfo.Id);
                        command.Parameters.AddWithValue("@Subject", SaleInfo.Subject);
                        command.Parameters.AddWithValue("@Body", SaleInfo.Body);
                        command.Parameters.AddWithValue("@DateSent", SaleInfo.DateSent.HasValue ? (object)SaleInfo.DateSent.Value : DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Record updated successfully.";
                            Response.Redirect($"/SaleD3/IndexSaleD3?view={viewType}"); // เด้งกลับไปยังหน้าเดิม (view=sent หรือ view=inbox)
                        }
                        else
                        {
                            ErrorMessage = "No record was updated. Please check the ID.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating data: {ex.Message}";
            }
        }



        public class SaleD3Info
        {
            public string Id { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public string ReceiverDepartment { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public DateTime? DateSent { get; set; }
        }
    }
}
