insert
	[dbo].[HistoryState]
(
	HistoryStateId
	,LastTemperatureReadingLsn
	,LastMoistureReadingLsn
	,LastSoilReadingLsn
	,LastStatusLsn
)
values
	(1, null, null, null, null); -- initial id to hold the states