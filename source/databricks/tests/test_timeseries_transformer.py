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
from package.schemas import time_series_schema
from package.transforms import JsonTransformer
from package.codelists import Colname
from datetime import datetime
import pandas as pd
import pytest


@pytest.fixture(scope="module")
def timeseries_result_factory(spark):
    def factory(
        metering_point_id=1,
        quantity=2,
        quality=3,
        time=datetime(2020, 1, 1, 0, 0)
    ):
        return spark.createDataFrame(pd.DataFrame().append([{
            Colname.metering_point_id: metering_point_id,
            Colname.quantity: quantity,
            Colname.quality: quality,
            Colname.time: time}],
            ignore_index=True))
    return factory


@pytest.fixture(scope="module")
def eventhub_timeseries_body_dataframe_factory(spark) -> pd.DataFrame:
    def factory(
        body='{"Document":{"Id":"C1876453","CreatedDateTime":"2022-12-17T09:30:47Z","Sender":{"Id":"5799999933317","BusinessProcessRole":1},"Receiver":{"Id":"5790001330552","BusinessProcessRole":2},"BusinessReasonCode":1},"Series":[{"Id":"C1876456","TransactionId":"C1875000","MeteringPointId":"579999993331812346","MeteringPointType":1,"RegistrationDateTime":"2022-12-17T07:30:00Z","Product":"8716867000030","MeasureUnit":1,"Period":{"Resolution":2,"StartDateTime":"2022-08-15T22:00:00Z","EndDateTime":"2022-08-15T04:00:00Z","Points":[{"Quantity":242,"Quality":3,"Position":1},{"Quantity":242,"Quality":4,"Position":2},{"Quantity":222,"Quality":4,"Position":3},{"Quantity":202,"Quality":4,"Position":4},{"Quantity":191,"Quality":5,"Position":5},{"Quantity":null,"Quality":2,"Position":6}]}},{"Id":"C1876456","TransactionId":"C1875000","MeteringPointId":"579999993331812345","MeteringPointType":1,"RegistrationDateTime":"2022-12-17T07:30:00Z","Product":"8716867000030","MeasureUnit":1,"Period":{"Resolution":2,"StartDateTime":"2022-08-15T22:00:00Z","EndDateTime":"2022-08-15T04:00:00Z","Points":[{"Quantity":242,"Quality":3,"Position":1},{"Quantity":242,"Quality":4,"Position":2},{"Quantity":222,"Quality":4,"Position":3},{"Quantity":202,"Quality":4,"Position":4}]}}]}'
    ):

        return spark.createDataFrame(pd.DataFrame().append([{'body': body}]))
    return factory


def test__createDataframe_from_json_input_schema(eventhub_timeseries_body_dataframe_factory):
    # Arrange
    transformer = JsonTransformer()

    # Act
    result = transformer.TransformFromJsonToDataframe(eventhub_timeseries_body_dataframe_factory())

    # Assert
    assert result.schema == time_series_schema
