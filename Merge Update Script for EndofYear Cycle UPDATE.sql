CREATE PROCEDURE [dbo].[spEndofYearReviewUpdateFeedbackCycles]
	@feedbackId int = null
	
AS
	merge dbo.FeedbackCycles fc
using (
		select * from dbo.FeedbackCycles fc1
		where
		fc1.FeedBackCycleId = (select fc2.FeedBackCycleId from dbo.FeedbackCycles fc2 where fc2.FeedBackCycleId = @feedbackId)
) srce
on (fc.FeedBackCycleId = srce.FeedBackCycleId)
when matched then
update set fc.EOYFeedbackCycleId = srce.FeedBackCycleId,
		   fc.MidYearFeedbackCycleId = null
;
RETURN 0