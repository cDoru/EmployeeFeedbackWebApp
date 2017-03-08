/*
	This script is intended to be run immediately after a new roy has been added to FeedbackCycles so that it can set the MidYearFeedbackCycleId column
	to be the same value as the new primary key that was just created.  This is to specify that the new row is a mid-year review period.
*/

merge EmployeeFeedbackSystem.dbo.FeedbackCycles fc
using (
		select * from EmployeeFeedbackSystem.dbo.FeedbackCycles
		where
		FeedBackCycleId = (select max(FeedBackCycleId) from EmployeeFeedbackSystem.dbo.FeedbackCycles)
) srce
on (fc.FeedbackCycleId = srce.FeedbackCycleId)
when matched then
update set fc.MidYearFeedbackCycleId = srce.FeedbackCycleId
;