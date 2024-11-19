using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace ProgrammerTuajew.Pages.ForgotPassword
{
    public class ForgotPasswordModel : PageModel
    {
        public string Message { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(NewPassword))
            {
                Message = "Please fill in all fields.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // ตรวจสอบว่าอีเมลมีอยู่ในระบบหรือไม่
                    string sqlCheck = "SELECT COUNT(*) FROM password WHERE Email = @Email";
                    using (SqlCommand commandCheck = new SqlCommand(sqlCheck, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@Email", Email);
                        int count = (int)commandCheck.ExecuteScalar();

                        if (count == 0)
                        {
                            Message = "Email address not found.";
                            return Page();
                        }
                    }

                    // อัพเดตรหัสผ่านใหม่ลงในตาราง `password`
                    string sqlUpdate = "UPDATE password SET Password = @NewPassword WHERE Email = @Email";
                    using (SqlCommand commandUpdate = new SqlCommand(sqlUpdate, connection))
                    {
                        commandUpdate.Parameters.AddWithValue("@Email", Email);
                        commandUpdate.Parameters.AddWithValue("@NewPassword", NewPassword);

                        commandUpdate.ExecuteNonQuery();
                        Message = "Password has been reset successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
            }

            return Page();
        }
    }
}
