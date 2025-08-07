variable "location" {
	type = string
	default = "Central US"
}

variable "resource_group_name" {
	type = string
	default = "FarmToTableResourceGroup"
}

variable "eventhub_namespace_name" {
	type = string
	default = "FarmToTableNamespace"
}

variable "eventhub_name" {
	type = string
	default = "FarmToTableEvents"
}

variable "storage_account_name" {
	type = string
	default = "farmtotablestorageacct"
}

variable "function_app_name" {
	type = string
	default = "FarmToTableFunctions"
}

variable "web_app_plan_name" {
	type = string
	default = "farmtotable-publisher-plan"
}

variable "web_app_name" {
	type = string
	default = "FarmToTablePublisherApp"
}

variable "key_vault_name" {
	type = string
	default = "FarmToTableKeyVault"
}

variable "sql_username" {
	type = string
	sensitive = true
	description = "SQL Server username"
}

variable "sql_password" {
	type = string
	sensitive = true
	description = "SQL Server password"
}

variable "eventhub_connection_string" {
	type = string
	sensitive = true
	description = "Event Hub Connection String"
}