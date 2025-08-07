GRANT SELECT ON dbo.Sentinel TO dbSimplot;
GRANT SELECT ON dbo.SentinelStatusHistory TO dbSimplot;
GRANT SELECT ON dbo.TemperatureReadingHistory TO dbSimplot;
GRANT SELECT ON dbo.MoistureReadingHistory TO dbSimplot;
GRANT SELECT ON dbo.SoilReadingHistory TO dbSimplot;
GO

GRANT SELECT ON cdc.[dbo_Sentinel_CT] TO dbSimplot;
GO

GRANT SELECT ON cdc.[dbo_SentinelStatusHistory_CT] TO dbSimplot;
GO

GRANT SELECT ON cdc.[dbo_TemperatureReadingHistory_CT] TO dbSimplot;
GO

GRANT SELECT ON cdc.[dbo_MoistureReadingHistory_CT] TO dbSimplot;
GO

GRANT SELECT ON cdc.[dbo_SoilReadingHistory_CT] TO dbSimplot;
GO

GRANT SELECT ON cdc.[fn_cdc_get_all_changes_dbo_Sentinel] TO dbSimplot;
GO

GRANT SELECT ON cdc.[fn_cdc_get_all_changes_dbo_SentinelStatusHistory] TO dbSimplot;
GO

GRANT SELECT ON cdc.[fn_cdc_get_all_changes_dbo_TemperatureReadingHistory] TO dbSimplot;
GO

GRANT SELECT ON cdc.[fn_cdc_get_all_changes_dbo_MoistureReadingHistory] TO dbSimplot;
GO

GRANT SELECT ON cdc.[fn_cdc_get_all_changes_dbo_SoilReadingHistory] TO dbSimplot;
GO

