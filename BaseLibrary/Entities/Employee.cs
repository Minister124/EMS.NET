namespace BaseLibrary.Entities
{
    public class Employee : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public string? FileNumber { get; set; }
        public string? FullName { get; set; }
        public string? JobName { get; set; }
        public string? Address { get; set; }
        public string? Telephone { get; set; }
        public string? Photo { get; set; }
        public string? Other { get; set; }
        public GeneralDepartment? GeneralDepartment { get; set; } //Many Employees belong to One Department
        public Guid GeneralDepartmentId { get; set; }
        public Department? Department { get; set; } //Many Employees belong to One Department or One department has many employees
        public Guid DepartmentId { get; set; }
        public Branch? Branch { get; set; }
        public Guid BranchId { get; set; } //same
        public Town? Town { get; set; }
        public Guid TownId { get; set; }
    }
}
