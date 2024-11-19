using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace ProgrammerTuajew.Pages.Register
{
    public class CustomRegisterModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }  // ข้อความสำหรับแสดงผลเมื่อเพิ่มข้อมูลสำเร็จ

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Department { get; set; } // เพิ่มฟิลด์ Department

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) || string.IsNullOrEmpty(Department))
            {
                ErrorMessage = "กรุณากรอกข้อมูลให้ครบทุกช่อง.";
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "รหัสผ่านไม่ตรงกัน.";
                return Page();
            }

            try
            {
                // เชื่อมต่อกับฐานข้อมูล โดยใช้ตาราง `password`
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // ตรวจสอบว่าอีเมลที่ลงทะเบียนซ้ำหรือไม่
                    string checkEmailSql = "SELECT COUNT(*) FROM password WHERE Email = @Email";
                    using (SqlCommand checkEmailCommand = new SqlCommand(checkEmailSql, connection))
                    {
                        checkEmailCommand.Parameters.AddWithValue("@Email", Email);
                        int count = (int)checkEmailCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            ErrorMessage = "อีเมลนี้ถูกใช้ไปแล้ว.";
                            return Page();
                        }
                    }

                    // เพิ่มข้อมูลลงในตาราง `password` พร้อมกับ Department
                    string sql = "INSERT INTO password (Email, Password, Department) VALUES (@Email, @Password, @Department)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@Department", Department);

                        command.ExecuteNonQuery();
                    }
                }

                // หลังจากสมัครเสร็จให้แสดงข้อความสำเร็จ
                SuccessMessage = "สมัครสมาชิกสำเร็จ!";
                return RedirectToPage("/Login/CustomLogin");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"เกิดข้อผิดพลาด: {ex.Message}";
            }

            return Page();
        }
    }
}
