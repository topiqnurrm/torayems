using System.ComponentModel.DataAnnotations;

namespace TorayEMS.Models
{
    /// <summary>
    /// Merepresentasikan tabel Employees di database.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Nama lengkap wajib diisi.")]
        [StringLength(150)]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jabatan wajib diisi.")]
        [StringLength(100)]
        [Display(Name = "Jabatan")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departemen wajib dipilih.")]
        [Display(Name = "Departemen")]
        public int DepartmentId { get; set; }

        // Diisi hasil JOIN dari stored procedure, hanya untuk ditampilkan (read-only)
        [Display(Name = "Departemen")]
        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "Tanggal masuk wajib diisi.")]
        [DataType(DataType.Date)]
        [Display(Name = "Tanggal Masuk")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Range(0, 999999999, ErrorMessage = "Gaji harus berupa angka positif.")]
        [Display(Name = "Gaji (IDR)")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Phone(ErrorMessage = "Format nomor telepon tidak valid.")]
        [StringLength(20)]
        [Display(Name = "No. Telepon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Status Aktif")]
        public bool IsActive { get; set; } = true;
    }
}
