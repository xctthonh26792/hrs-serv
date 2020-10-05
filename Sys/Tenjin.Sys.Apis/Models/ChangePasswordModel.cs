namespace Tenjin.Sys.Apis.Models
{
    public class ChangePasswordModel
    {
        public string Code { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
