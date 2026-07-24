namespace TorayEMS.Models
{
    /// <summary>
    /// ViewModel gabungan untuk menampilkan ringkasan data di halaman utama (dashboard).
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalActiveEmployees { get; set; }
        public List<Department> EmployeeCountByDepartment { get; set; } = new();
    }
}
