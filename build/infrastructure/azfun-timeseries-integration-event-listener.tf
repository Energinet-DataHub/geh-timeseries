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
module "azfun_integration_event_listener" {
  source                                    = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//function-app?ref=1.2.0"
  name                                      = "azfun-integration-event-listener-${var.project}-${var.organisation}-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.main.name
  location                                  = data.azurerm_resource_group.main.location
  storage_account_access_key                = module.azfun_ingestion_event_listener_stor.primary_access_key
  app_service_plan_id                       = module.azfun_ingestion_event_listener_plan.id
  storage_account_name                      = module.azfun_ingestion_event_listener_stor.name
  application_insights_instrumentation_key  = module.appi.instrumentation_key
  tags                                      = data.azurerm_resource_group.main.tags
  app_settings                              = {
    # Region: Default Values
    WEBSITE_ENABLE_SYNC_UPDATE_SITE                       = true
    WEBSITE_RUN_FROM_PACKAGE                              = 1
    WEBSITES_ENABLE_APP_SERVICE_STORAGE                   = true
    FUNCTIONS_WORKER_RUNTIME                              = "dotnet-isolated"
    CACERT_PATH                                           = var.cacert_path
    LOCAL_TIMEZONENAME                                    = local.LOCAL_TIMEZONENAME
    INTEGRATION_EVENT_LISTENER_CONNECTION_STRING          = "to be set manually - for now"
    CONSUMPTION_METERING_POINT_CREATED_TOPIC_NAME         = "consumption-metering-point-created"
    CONSUMPTION_METERING_POINT_CREATED_SUBSCRIPTION_NAME  = "consumption-metering-point-created-to-timeseries"
    METERING_POINT_CONNECTED_TOPIC_NAME                   = "metering-point-connected"
    METERING_POINT_CONNECTED_SUBSCRIPTION_NAME            = "metering-point-connected-to-timeseries"
    EVENT_HUB_CONNECTION                                  = module.evhar_ingestion_queue_sender.primary_connection_string
    EVENT_HUB_NAME                                        = module.evh_ingestion_queue.name
  }
  dependencies                              = [
    module.appi.dependent_on,
    module.azfun_ingestion_event_listener_plan.dependent_on,
    module.azfun_ingestion_event_listener_stor.dependent_on,
  ]
}

module "azfun_ingestion_event_listener_plan" {
  source              = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//app-service-plan?ref=1.2.0"
  name                = "asp-ingestion-event-listener-${var.project}-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Free"
    size  = "F1"
  }
  tags                = data.azurerm_resource_group.main.tags
}

module "azfun_ingestion_event_listener_stor" {
  source                    = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//storage-account?ref=1.7.0"
  name                      = "stormsgrcvr${random_string.ingestion_event_listener.result}"
  resource_group_name       = data.azurerm_resource_group.main.name
  location                  = data.azurerm_resource_group.main.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.main.tags
}

# Since all functions need a storage connected we just generate a random name
resource "random_string" "ingestion_event_listener" {
  length  = 6
  special = false
  upper   = false
}

# module "ping_webtest_ingestion_event_listener" {
#  source                          = "./modules/ping-webtest" # Repo geh-terraform-modules doesn't have a webtest module at the time of this writing
#  name                            = "ping-webtest-ingestion-event-listener-${var.project}-${var.organisation}-${var.environment}"
#  resource_group_name             = data.azurerm_resource_group.main.name
#  location                        = data.azurerm_resource_group.main.location
#  tags                            = data.azurerm_resource_group.main.tags
#  application_insights_id         = module.appi.id
#  url                             = "https://${module.azfun_ingestion_event_listener.default_hostname}/api/HealthStatus"
#  dependencies                    = [module.azfun_ingestion_event_listener.dependent_on]
#}