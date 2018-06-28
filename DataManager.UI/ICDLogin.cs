using DataManager.Core;
using System;

namespace DataManager.UI
{
    public class ICDLogin
    {
        [PrimaryKey]
		public long Id { get; set; }
        public string MemberName { get; set; }
        public string MemberNo { get; set; }
        public string OldMemberNo { get; set; }
        public string Password { get; set; }
        public int UserRole { get; set; }
        public string ProposedBy { get; set; }
        public string ProposedMemberNo { get; set; }
        public DateTime ProposedMemberSince { get; set; }
        public string Status { get; set; }
        public string UniqueId { get; set; }
        public string IPAddress { get; set; }
        public DateTime Since { get; set; }
    }
}
