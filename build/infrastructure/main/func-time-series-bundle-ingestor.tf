# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
module "time_series_bundle_ingestor" {
  source                                    = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//azure/function-app?ref=7.0.0"

  name                                      = "time-series-bundle-ingestor"
  project_name                              = var.domain_name_short
  environment_short                         = var.environment_short
  environment_instance                      = var.environment_instance
  resource_group_name                       = azurerm_resource_group.this.name
  location                                  = azurerm_resource_group.this.location
  vnet_integration_subnet_id                = data.azurerm_key_vault_secret.snet_vnet_integrations_id.value
  private_endpoint_subnet_id                = data.azurerm_key_vault_secret.snet_private_endpoints_id.value
  app_service_plan_id                       = data.azurerm_key_vault_secret.plan_shared_id.value
  application_insights_instrumentation_key  = data.azurerm_key_vault_secret.appi_shared_instrumentation_key.value
  log_analytics_workspace_id                = data.azurerm_key_vault_secret.log_shared_id.value
  always_on                                 = true
  health_check_path                         = "/api/monitor/ready"
  health_check_alert_action_group_id        = data.azurerm_key_vault_secret.primary_action_group_id.value
  health_check_alert_enabled                = var.enable_health_check_alerts
  dotnet_framework_version                  = "6"
  use_dotnet_isolated_runtime               = true
  app_settings                              = {
    DATA_LAKE_ACCOUNT_NAME                              = data.azurerm_key_vault_secret.st_shared_data_lake_name.value
    DATA_LAKE_CONNECTION_STRING                         = data.azurerm_key_vault_secret.kvs_st_data_lake_primary_connection_string.value
    DATA_LAKE_KEY                                       = data.azurerm_key_vault_secret.kvs_st_data_lake_primary_access_key.value
    REQUEST_RESPONSE_LOGGING_CONNECTION_STRING          = "@Microsoft.KeyVault(VaultName=${var.shared_resources_keyvault_name};SecretName=st-marketoplogs-primary-connection-string)",
    REQUEST_RESPONSE_LOGGING_CONTAINER_NAME             = "@Microsoft.KeyVault(VaultName=${var.shared_resources_keyvault_name};SecretName=st-marketoplogs-container-name)",
    B2C_TENANT_ID                                       = "@Microsoft.KeyVault(VaultName=${var.shared_resources_keyvault_name};SecretName=b2c-tenant-id)",
    BACKEND_SERVICE_APP_ID                              = "@Microsoft.KeyVault(VaultName=${var.shared_resources_keyvault_name};SecretName=backend-service-app-id)",
    EVENT_HUB_CONNECTION_STRING                         = module.evh_received_timeseries.primary_connection_strings["send"]
    EVENT_HUB_NAME                                      = module.evh_received_timeseries.name
    "TimeSeriesRawFolder:FolderName"                    = local.DATA_LAKE_TIME_SERIES_RAW_FOLDER_NAME
    DATA_LAKE_CONTAINER_NAME                            = local.DATA_LAKE_CONTAINER_NAME
  }

  tags                                      = azurerm_resource_group.this.tags
}