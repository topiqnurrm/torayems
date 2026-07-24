using System.ComponentModel.DataAnnotations;

namespace TorayEMS.Models
{
    /// <summary>
    /// Merepresentasikan tabel Departments di database.
    /// </summary>
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Nama departemen wajib diisi.")]
        [StringLength(100)]
        [Display(Name = "Nama Departemen")]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Digunakan untuk menampilkan jumlah karyawan per departemen di dashboard
        public int EmployeeCount { get; set; }
    }
}
