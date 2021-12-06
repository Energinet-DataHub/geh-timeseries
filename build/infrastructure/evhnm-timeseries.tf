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

#EventHub namespace for EventHubs in the TimeSeries domain
module "evhnm_timeseries" {
  source                    = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//event-hub-namespace?ref=2.0.0"
  name                      = "evhnm-timeseries-${var.project}-${var.organisation}-${var.environment}"
  resource_group_name       = data.azurerm_resource_group.main.name
  location                  = data.azurerm_resource_group.main.location
  sku                       = "Standard"
  capacity                  = 1
  tags                      = data.azurerm_resource_group.main.tags
}
