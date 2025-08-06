exec sys.sp_cdc_enable_db;
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