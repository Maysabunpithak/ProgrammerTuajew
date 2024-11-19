using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;

namespace ProgrammerTuajew.Pages.Register
{
    public class CustomRegisterModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }  // Message for successful registration

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Department { get; set; } // Department field

        [BindProperty]
        public string PhoneNumber { get; set; } // Phone number field

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) || string.IsNullOrEmpty(Department) || string.IsNullOrEmpty(PhoneNumber))
            {
                ErrorMessage = "Please fill in all fields.";
                return Page();
            }

            // Validate phone number: must be 10 digits (xxxxxxxxxx)
            if (!Regex.IsMatch(PhoneNumber, @"^\d{10}$"))
            {
                ErrorMessage = "Phone number must be exactly 10 digits (format: xxxxxxxxxx) and contain only numbers.";
                return Page();
            }

            // Validate Email: must not be only numbers, must include letters, and no whitespace allowed
            if (Regex.IsMatch(Email, @"^\d+$"))
            {
                ErrorMessage = "Email must include letters and cannot be only numbers.";
                return Page();
            }
            if (Email.Contains(" "))
            {
                ErrorMessage = "Email cannot contain spaces.";
                return Page();
            }
            if (!Email.Contains("@example"))
            {
                ErrorMessage = "Email must follow the correct format (e.g., user@example.com).";
                return Page();
            }

            // Validate Password: no whitespace allowed
            if (Password.Contains(" "))
            {
                ErrorMessage = "Password cannot contain spaces.";
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            try
            {
                // Connect to the database using the `password` table
                string connectionString = "Server=tcp:datacs436.database.windows.net,1433;Initial Catalog=databaseCs436;Persist Security Info=False;User ID=maysa;Password=Masa1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the email is already registered
                    string checkEmailSql = "SELECT COUNT(*) FROM password WHERE Email = @Email";
                    using (SqlCommand checkEmailCommand = new SqlCommand(checkEmailSql, connection))
                    {
                        checkEmailCommand.Parameters.AddWithValue("@Email", Email);
                        int count = (int)checkEmailCommand.ExecuteScalar();
                        if (count > 0)
                        {
                            ErrorMessage = "This email is already registered.";
                            return Page();
                        }
                    }

                    // Insert data into the `password` table along with Department and PhoneNumber
                    string sql = "INSERT INTO password (Email, Password, Department, PhoneNumber) VALUES (@Email, @Password, @Department, @PhoneNumber)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@Department", Department);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

                        command.ExecuteNonQuery();
                    }
                }

                // Show success message after registration
                SuccessMessage = "Registration successful!";
                return RedirectToPage("/Login/CustomLogin");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return Page();
        }
    }
}
