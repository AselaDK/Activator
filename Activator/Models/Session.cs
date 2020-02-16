namespace Activator.Models
{

    public class Session
    {
        public static string id;
        public static bool status;
        public bool MyStatus { get; set; }
        public string MyId { get; set; }
        public Session()
        {
            id = MyId;
            status = MyStatus;
        }
    }
}
