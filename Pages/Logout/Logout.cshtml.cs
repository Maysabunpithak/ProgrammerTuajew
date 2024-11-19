using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProgrammerTuajew.Pages.Logout
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear(); // ลบข้อมูลทั้งหมดใน Session เพื่อออกจากระบบ
            return RedirectToPage("/Index"); // กลับไปยังหน้าแรกหลัง Logout
        }
    }
}