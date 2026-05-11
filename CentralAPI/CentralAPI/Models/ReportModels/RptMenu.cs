namespace CentralAPI.Models.ReportModels
{
    public class RptMenu
    {
        #region Public Properties         
        public string ModuleName { get; set; }
        public string ParentName { get; set; }

        public string ModuleIcon { get; set; }
        public string ModuleLblName { get; set; }
        public string ModuleLblIcon { get; set; }
        public int? ModuleSorting { get; set; }
        public string ActiveClass { get; set; }
        public int MenuId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public string DisplayName { get; set; }
        public string ActionResult { get; set; }
        public string Controller { get; set; }
        public string MenuIcon { get; set; }
        public string MenuLblName { get; set; }
        public string MenuLblIcon { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActiveMenu { get; set; }
        public string ProjectName { get; set; }
        public int? CompId { get; set; }
        public bool? Access { get; set; }
        #endregion
    }
}
