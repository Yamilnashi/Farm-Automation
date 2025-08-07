insert
	[dbo].[HistoryState]
(
	HistoryStateId
	,LastTemperatureReadingLsn
	,LastMoistureReadingLsn
	,LastSoilReadingLsn
	,LastStatusLsn
	,LastSentinelLsn
)
values
	(1, null, null, null, null, null); -- initial id to hold the states