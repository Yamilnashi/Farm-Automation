exec sys.sp_cdc_disable_table
  @source_schema = N'dbo',
  @source_name = N'Sentinel',
  @capture_instance = 'dbo_Sentinel'
go

exec sys.sp_cdc_disable_table
  @source_schema = N'dbo',
  @source_name = N'SentinelStatusHistory',
  @capture_instance = 'dbo_SentinelStatusHistory'
go

exec sys.sp_cdc_disable_table
  @source_schema = N'dbo',
  @source_name = N'TemperatureReadingHistory',
  @capture_instance = 'dbo_TemperatureReadingHistory'
go

exec sys.sp_cdc_disable_table
  @source_schema = N'dbo',
  @source_name = N'MoistureReadingHistory',
  @capture_instance = 'dbo_MoistureReadingHistory'
go

exec sys.sp_cdc_disable_table
  @source_schema = N'dbo',
  @source_name = N'SoilReadingHistory',
  @capture_instance = 'dbo_SoilReadingHistory'
go

exec sys.sp_cdc_enable_table
  @source_schema = N'dbo',
  @source_name = N'Sentinel',
  @role_name = null,
  @supports_net_changes = 1;
go

exec sys.sp_cdc_enable_table
  @source_schema = N'dbo',
  @source_name = N'SentinelStatusHistory',
  @role_name = null,
  @supports_net_changes = 1;
go

exec sys.sp_cdc_enable_table
  @source_schema = N'dbo',
  @source_name = N'TemperatureReadingHistory',
  @role_name = null,
  @supports_net_changes = 1;
go

exec sys.sp_cdc_enable_table
  @source_schema = N'dbo',
  @source_name = N'MoistureReadingHistory',
  @role_name = null,
  @supports_net_changes = 1;
go

exec sys.sp_cdc_enable_table
  @source_schema = N'dbo',
  @source_name = N'SoilReadingHistory',
  @role_name = null,
  @supports_net_changes = 1;
go

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