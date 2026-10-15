using System.ComponentModel.DataAnnotations.Schema;

namespace BMSAPI.Models.Common
{
    public class UserProfile : Base
    {
        #region Public Properties  

        public int UserAccountId { set; get; }
        [ForeignKey("UserAccountId")]
        
        public string? userid { get; set; }
        public string fullname { get; set; }      
        public string? Class { get; set; }
        public string? grade { get; set; }
        public string? filepath { get; set; }
        public string? filename { get; set; }        
       
        #endregion
    }
}
