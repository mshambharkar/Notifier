
using System.Collections.Generic;

namespace WPFNotification.Model
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public List<string> Messages { get; set; }
        public string ImgURL { get; set; }
    }
}
