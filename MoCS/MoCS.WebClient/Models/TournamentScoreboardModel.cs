using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoCS.Business.Objects;
using System.Collections.Specialized;

namespace MoCS.WebClient.Models
{
    public class StatusInfo
    {
        public long SecondsSinceEnrollment { get; set; }

        public StatusEnum Status { get; set; }

        public int FinishOrder { get; set; }
    }

    public enum StatusEnum
    {
        Started,
        Failed,
        Finished,
        NotStarted
    }

    public class TournamentScoreboardModel
    {
        public List<Team> Teams { get; set; }

        public List<Assignment> Assignments { get; set; }

        private Dictionary<string, StatusInfo> _submitInfos = new Dictionary<string, StatusInfo>();

        public OrderedDictionary StatusInfosPerTeam = new OrderedDictionary();
 
        public void Fill(List<TournamentAssignment> taList)
        {
            // Get the teams
            Teams = new List<Team>();
            Assignments = new List<Assignment>();

            OrderedDictionary tempStatusInfosPerTeam = new OrderedDictionary();

            OrderedDictionary submitsPerTournamentAssignment = new OrderedDictionary();

            foreach (TournamentAssignment ta in taList)
            {
                submitsPerTournamentAssignment.Add(ta.Id.ToString(), new List<Submit>());

                Assignments.Add(ta.Assignment);

                foreach (AssignmentEnrollment ae in ta.AssignmentEnrollmentList)
                {
                    // Add the team for this enrollment to the list
                    if (!Teams.Exists(t => t.Id == ae.Team.Id))
                    {
                        Teams.Add(ae.Team);
                        tempStatusInfosPerTeam.Add(ae.Team.Id.ToString(), new List<StatusInfo>());
                    }
                }
            }

            foreach (Team team in Teams)
            {
                foreach (TournamentAssignment ta in taList)
                {
                    // check if the team has an enrollment for this tournamentassignment
                    AssignmentEnrollment ae = ta.AssignmentEnrollmentList.Find(tae => tae.Team.Id == team.Id);
                    if (ae != null)
                    {
                        //If there's a submit, add it to the dictionary
                        if (ae.SubmitList.Count > 0)
                        {
                            ((List<Submit>)submitsPerTournamentAssignment[ta.Id.ToString()]).Add(ae.SubmitList[0]);
                        }
                        else
                        {
                            //add an empty submit placeholder for this enrollment
                            ((List<Submit>)submitsPerTournamentAssignment[ta.Id.ToString()]).Add(new Submit()
                            {
                                Status = "NullSubmit",
                                Team = ae.Team,
                                SecondsSinceEnrollment = 0
                            });
                        }
                    }
                    else
                    {
                        //add an empty submit placeholder for this enrollment
                        ((List<Submit>)submitsPerTournamentAssignment[ta.Id.ToString()]).Add(new Submit()
                        {
                            Status = "NullSubmit",
                            Team = team,
                            SecondsSinceEnrollment = 0
                        });
                    }
                }
            }

            foreach (TournamentAssignment ta in taList)
            {
                // sort the submits descending
                ((List<Submit>)submitsPerTournamentAssignment[ta.Id.ToString()]).Sort(new DescendingSubmitComparer());
            }

            foreach (string taId in submitsPerTournamentAssignment.Keys)
            {
                int finishOrder = 1;
                foreach (Submit s in ((List<Submit>)submitsPerTournamentAssignment[taId]))
                {
                    StatusEnum convertedStatus;
                    convertedStatus = ConvertSubmitStatus(s.ConvertStatus(s.Status));

                    StatusInfo statusInfo = new StatusInfo();
                    statusInfo.SecondsSinceEnrollment = s.SecondsSinceEnrollment;
                    statusInfo.Status = convertedStatus;
                    statusInfo.FinishOrder = 0;

                    // If status is finished (successfull submit) add the seconds since enrollment
                    // to the score of the team
                    if (convertedStatus == StatusEnum.Finished)
                    {
                        Teams.Find(t => t.Id == s.Team.Id).Score += s.SecondsSinceEnrollment;
                        statusInfo.FinishOrder = finishOrder;
                        finishOrder++;
                    }

                    ((List<StatusInfo>)tempStatusInfosPerTeam[s.Team.Id.ToString()]).Add(statusInfo);
                }
            }

            Dictionary<int, List<Team>> FinishedPerTeam = new Dictionary<int, List<Team>>();
            List<int> finishedCounts = new List<int>();

            foreach (string teamId in tempStatusInfosPerTeam.Keys)
            {
                // determine how many assignments the team has finished
                int nrOfFinished = ((List<StatusInfo>)tempStatusInfosPerTeam[teamId]).FindAll(st => st.Status == StatusEnum.Finished).Count;

                if (!finishedCounts.Exists(f => f == nrOfFinished))
                {
                    finishedCounts.Add(nrOfFinished);
                    FinishedPerTeam.Add(nrOfFinished, new List<Team>());
                }

                if (nrOfFinished > 1)
                {
                    long totalScore = Teams.Find(t => t.Id == int.Parse(teamId)).Score;

                    // calculate mean score
                    long meanScore = (long)Math.Round((double)totalScore / (double)nrOfFinished);
                    Teams.Find(t => t.Id == int.Parse(teamId)).Score = meanScore;
                }

                FinishedPerTeam[nrOfFinished].Add(Teams.Find(t => t.Id == int.Parse(teamId)));
            }

            finishedCounts.Sort();
            finishedCounts.Reverse();

            List<Team> sortedTeamList = new List<Team>();

            foreach (int finishedCount in finishedCounts)
            {
                List<Team> teams = FinishedPerTeam[finishedCount];
                teams.Sort(new ScoreComparer());
                teams.Reverse();
                sortedTeamList.AddRange(teams);
            }

            Teams = sortedTeamList;

            //build definite statusInfosPerTeam dictionary, sorting by team
            foreach (Team t in Teams)
            {
                StatusInfosPerTeam.Add(t.Id.ToString(), tempStatusInfosPerTeam[t.Id.ToString()]);
            }
        }

        public StatusEnum ConvertSubmitStatus(SubmitStatus submitStatus)
        {
            switch (submitStatus)
            {
                case SubmitStatus.Success:
                    return StatusEnum.Finished;
                case SubmitStatus.Submitted:
                case SubmitStatus.Processing:
                    return StatusEnum.Started;
                case SubmitStatus.NullSubmit:
                    return StatusEnum.NotStarted;
                case SubmitStatus.ErrorCompilation:
                case SubmitStatus.ErrorValidation:
                case SubmitStatus.ErrorTesting:
                case SubmitStatus.ErrorServer:
                case SubmitStatus.ErrorUnknown:
                    return StatusEnum.Failed;
                default:
                    break;
            }

            return StatusEnum.NotStarted;
        }

        public class DescendingSubmitComparer : IComparer<Submit>
        {
            public int Compare(Submit x, Submit y)
            {
                SubmitStatus xStatus = x.ConvertStatus(x.Status);
                SubmitStatus yStatus = y.ConvertStatus(y.Status);

                if (x.ConvertStatus(x.Status) == y.ConvertStatus(y.Status))
                {
                    if (x.SecondsSinceEnrollment == y.SecondsSinceEnrollment)
                    {
                        return 0;
                    }

                    if (x.SecondsSinceEnrollment < y.SecondsSinceEnrollment)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                if (x.ConvertStatus(x.Status) < y.ConvertStatus(y.Status))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public class ScoreComparer : IComparer<Team>
        {
            public int Compare(Team x, Team y)
            {
                if (x.Score == y.Score)
                {
                    return 0;
                }

                if (x.Score != 0 && y.Score != 0)
                {
                    if (x.Score > y.Score)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (x.Score == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        private string CreateKey(Team team, Assignment assignment)
        {
            return team.Id.ToString() + "_" + assignment.Id.ToString();
        }

        public void SetInfo(Team team, Assignment assignment, StatusInfo info)
        {
            string key = CreateKey(team, assignment);
            if (_submitInfos.ContainsKey(key))
            {
                _submitInfos[key] = info;
            }
            else
            {
                _submitInfos.Add(key, info);
            }
        }
    }
}