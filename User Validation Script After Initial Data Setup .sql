select usr.FirstName,
	usr.LastName,
	fc.FeedbackCycleName,
	/*mid.MidPerfIndicator,
	mid.MidCommentsPositive,
	mid.MidCommentsImprove */
	eoy.EOYOverallPerf,
	eoy.EOYCommentsPositive,
	eoy.EOYCommentsImprove
from EmployeeFeedbackSystem.dbo.Users usr
join EmployeeFeedbackSystem.dbo.FeedbackAssignments fa on usr.UserId = fa.UserId
join EmployeeFeedbackSystem.dbo.FeedbackCycles fc on fa.FeedbackCycleId = fc.FeedBackCycleId
--join EmployeeFeedbackSystem.dbo.MidYearFeedback mid on fc.FeedBackCycleId = mid.MidYearFeedbackCycleId and fa.FeedbackAssignmentId = mid.MidYearFeedbackAssignmentId
join EmployeeFeedbackSystem.dbo.EndOfYearFeedback eoy on fc.FeedBackCycleId = eoy.EOYFeedbackCycleId and fa.FeedbackAssignmentId = eoy.EOYFeedbackAssignmentId
where
usr.UserStatus = 1 -- Active users only
and usr.RoleId = 3 --Regular Employees only
and fc.IsActive = 'Active' --Active review cycles only
and fc.EOYFeedbackCycleId = fc.FeedBackCycleId --EOY Review Cycles Only
--and fc.MidYearFeedbackCycleId = fc.FeedBackCycleId -- Mid-Year Review Cycles Only


select usr.FirstName,
	usr.LastName,
	r.RoleName
	from EmployeeFeedbackSystem.dbo.Users usr
	join EmployeeFeedbackSystem.dbo.Role r on usr.RoleId = r.RoleId



