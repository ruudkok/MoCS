using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.MobileControls;

namespace MoCS.WebClient.Models
{
    public class TournamentAssignmentModel
    {
        public int Id { get; set; }

        public int AssignmentId { get; set; }

        public string Tagline { get; set; }

        public string AssignmentName { get; set; }

        public int Difficulty { get; set; }

        public int Points { get; set; }

        public string FriendlyName { get; set; }

        public string Author { get; set; }

        public string Category { get; set; }

        public bool IsActive { get; set; }
    }

    public class TournamentAssignmentsModel : List<TournamentAssignmentModel> 
    {
    }
}
