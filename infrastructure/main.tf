resource "azurerm_resource_group" "rg" {
	name = var.resource_group_name
	location = var.location
}

resource "azurerm_eventhub_namespace" "eh_namespace" {
	name = var.eventhub_namespace_name
	location = azurerm_resource_group.rg.location
	resource_group_name = azurerm_resource_group.rg.name
	sku = "Standard"
	capacity = 1
}

resource "azurerm_eventhub" "eh" {
	name = var.eventhub_name
	namespace_name = azurerm_eventhub_namespace.eh_namespace.name
	resource_group_name = azurerm_resource_group.rg.name
	partition_count = 2
	message_retention = 1
}

resource "azurerm_storage_account" "storage" {
	name = var.storage_account_name
	resource_group_name = azurerm_resource_group.rg.name
	location = azurerm_resource_group.rg.location
	account_tier = "Standard"
	account_replication_type = "LRS"
}

resource "azurerm_storage_container" "container" {
	name = "functions-state"
	storage_account_name = azurerm_storage_account.storage.name
	container_access_type = "private"
}

resource "azurerm_service_plan" "plan" {
	name = "${var.function_app_name}-plan"
	location = azurerm_resource_group.rg.location
	resource_group_name = azurerm_resource_group.rg.name
	os_type = "Windows"
	sku_name = "Y1"
}

resource "azurerm_windows_function_app" "functions" {
	name = var.function_app_name
	location = azurerm_resource_group.rg.location
	resource_group_name = azurerm_resource_group.rg.name
	service_plan_id = azurerm_service_plan.plan.id
	storage_account_name = azurerm_storage_account.storage.name
	storage_account_access_key = azurerm_storage_account.storage.primary_access_key
	site_config {}
}

resource "azurerm_service_plan" "publisher_plan" {
	name = var.publisher_plan_name
	location = azurerm_resource_group.rg.location
	resource_group_name = azurerm_resource_group.rg.name
	os_type = "Windows"
	sku_name = "B1"
}

resource "azurerm_windows_web_app" "publisher_app" {
	name = var.web_app_name
	location = azurerm_resource_group.rg.location
	resource_group_name = azurerm_resource_group.rg.name
	service_plan_id = azurerm_service_plan.publisher_plan.id
	site_config {
		application_stack {
		dotnet_version = "v8.0"
		}
	}
}

resource "azurerm_key_vault" "kv" {
	name = var.key_vault_name
	location = azurerm_resource_group.rg.location
	resource_group_name = azurerm_resource_group.rg.name
	enabled_for_disk_encryption = true
	tenant_id = data.azurerm_client_config.current.tenant_id
	soft_delete_retention_days = 7
	purge_protection_enabled = false
	sku_name = "standard"
}

resource "azurerm_key_vault_access_policy" "user_policy" {
	key_vault_id = azurerm_key_vault.kv.id
	tenant_id = data.azurerm_client_config.current.tenant_id
	object_id = data.azurerm_client_config.current.object_id
	secret_permissions = [
		"Get",
		"List",
		"Set"
	]
}

resource "azurerm_key_vault_secret" "sql_username" {
	name = "SqlUsername"
	value = var.sql_username
	key_vault_id = azurerm_key_vault.kv.id
	depends_on = [azurerm_key_vault_access_policy.user_policy]
}

resource "azurerm_key_vault_secret" "sql_password" {
	name = "SqlPassword"
	value = var.sql_password
	key_vault_id = azurerm_key_vault.kv.id
	depends_on = [azurerm_key_vault_access_policy.user_policy]
}

data "azurerm_client_config" "current" {}