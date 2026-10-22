namespace BMSAPI.Models.Common
{
	public class ACC_GetChartOfAccounts
	{
		public int GLedgerId { get; set; }
		public string GLedgerCode { get; set; }
		public string GLedgerDescription { get; set; }
		public string GLedgerAccType { get; set; }

		// Internal Ledger properties
		public int ILedgerId { get; set; }
		public string ILedgerCode { get; set; }
		public string ILedgerDescription { get; set; }

		public int? PGLedger { get;set; }
	}
}
