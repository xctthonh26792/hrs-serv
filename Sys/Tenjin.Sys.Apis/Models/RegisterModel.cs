namespace Tenjin.Sys.Apis.Models
{
    public class RegisterModel
    {
        public string Code { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public int Permission { get; set; }
    }
}
