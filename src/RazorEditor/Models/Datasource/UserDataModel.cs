namespace RazorEditor.Models.Datasource
{
    public class UserDataModel : BaseDatasourceModel
    {
        public bool IsActive { get; set; }
        public bool IsCIActive { get; set; }
        public string UserName { get; set; }
        public string WindowsUserName { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? ProviderId { get; set; }
        public string PhoneExtension { get; set; }
        public string Theme { get; set; }
        public string MenuLocation { get; set; }
        public int? DepartmentId { get; set; }
        public string Title { get; set; }
    }
}