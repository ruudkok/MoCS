using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.Business.Objects;

namespace MoCS.Business.Objects.Interfaces
{
    public interface IBuildServiceFacade
    {
        List<Submit> GetUnprocessedSubmits();
        void UpdateSubmitStatusDetails(int submitId, SubmitStatus newStatus, string details, DateTime statusDate);
        Assignment GetAssignmentById(int assignmentId, bool includeServerFiles);
    }
}
