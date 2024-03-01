namespace LeadManagementSystem.Models
{
    public class LeadStatus
    {

        public int Id { get; set; }

        public int LeadId { get; set; }

        public String? Remark { get; set; }

        public DateTime DateTime {  get; set; }

        public String Status {  get; set; }

        public string AgentId {get; set; }
        public string AgentName { get; set; }

    }
}
