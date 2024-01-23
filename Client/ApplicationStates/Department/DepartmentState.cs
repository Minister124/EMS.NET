namespace Client.ApplicationStates.Department
{
    public class DepartmentState
    {
        public Action? GeneralDepartmentAction { get; set; }
        public bool ShowGeneralDepartment { get; set; }

        public void GeneralDepart()
        {
            ResetAllDepart();
            ShowGeneralDepartment = true;
            GeneralDepartmentAction?.Invoke();
        }

        private void ResetAllDepart()
        {
            ShowGeneralDepartment = false;
        }
    }
}
