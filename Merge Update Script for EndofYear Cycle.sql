/*
	This script is intended to be run immediately after a new row has been added to FeedbackCycles so that it can set the EOYFeedbackCycleId column
	to be the same value as the new primary key that was just created.  This is to specify that the new row is an end-of-year review period.
*/

	merge dbo.FeedbackCycles fc
using (
		select * from dbo.FeedbackCycles fc1
		where
		fc1.FeedBackCycleId = (select max(fc2.FeedBackCycleId) from dbo.FeedbackCycles fc2)
) srce
on (fc.FeedBackCycleId = srce.FeedBackCycleId)
when matched then
update set fc.EOYFeedbackCycleId = srce.FeedBackCycleId
;