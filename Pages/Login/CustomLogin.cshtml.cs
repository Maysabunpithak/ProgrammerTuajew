using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ProgrammerTuajew.Pages.CustomLogin
{
    public class LoginModel : PageModel
    {
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Department { get; set; } // เพิ่มฟิลด์ Department

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Department))
            {
                ErrorMessage = "Please fill in all fields.";
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Department FROM password WHERE Email = @Email AND Password = @Password AND Department = @Department";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@Department", Department);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string department = reader["Department"].ToString();

                                // บันทึกข้อมูลผู้ใช้ลงใน Session
                                HttpContext.Session.SetString("UserEmail", Email);
                                HttpContext.Session.SetString("UserDepartment", department);

                                // รีไดเร็กไปยังหน้าแผนกที่ผู้ใช้มีสิทธิ์และแสดงหน้า Inbox โดยอัตโนมัติ
                                if (department == "SaleD1") return RedirectToPage("/SaleD1/IndexSaleD1", new { view = "inbox" });
                                if (department == "SaleD2") return RedirectToPage("/SaleD2/IndexSaleD2", new { view = "inbox" });
                                if (department == "SaleD3") return RedirectToPage("/SaleD3/IndexSaleD3", new { view = "inbox" });
                                if (department == "Warehouse") return RedirectToPage("/Warehouse/IndexWarehouse", new { view = "inbox" });
                            }
                            else
                            {
                                ErrorMessage = "Invalid email, password, or department.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return Page();
        }
    }
}
