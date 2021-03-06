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
from pyspark.sql import SparkSession
from pyspark.sql.types import StringType
from pyspark.sql import DataFrame

from .event_meta_data import EventMetaData


def __process_eventhub_item(df: DataFrame, epoch_id, events_delta_path):
    if len(df.head(1)) > 0:
        # Extract metadata from the eventhub message and wrap into containing dataframe
        jsonDataFrame = df.select(
            (df.properties[EventMetaData.event_id].alias(EventMetaData.event_id)),
            (df.properties[EventMetaData.processed_date].alias(EventMetaData.processed_date)),
            (df.properties[EventMetaData.event_name].alias(EventMetaData.event_name)),
            (df.properties[EventMetaData.domain].alias(EventMetaData.domain)),
            (df.body.cast(StringType()).alias("body")))

        jsonDataFrame.show(truncate=False)

        # Append event
        jsonDataFrame.write \
            .partitionBy(EventMetaData.event_name) \
            .format("delta") \
            .mode("append") \
            .save(events_delta_path)


def ingest_events(event_hub_connection_key: str, delta_lake_container_name: str, storage_account_name: str, events_delta_path):

    spark = SparkSession.builder.getOrCreate()

    input_configuration = {}
    input_configuration["eventhubs.connectionString"] = spark.sparkContext._gateway.jvm.org.apache.spark.eventhubs.EventHubsUtils.encrypt(event_hub_connection_key)
    streamingDF = (spark.readStream.format("eventhubs").options(**input_configuration).load())

    checkpoint_path = f"abfss://{delta_lake_container_name}@{storage_account_name}.dfs.core.windows.net/events_checkpoint"
    stream = (streamingDF.writeStream.option("checkpointLocation", checkpoint_path).foreachBatch(lambda df, epochId: __process_eventhub_item(df, epochId, events_delta_path)).start())
    stream.awaitTermination()
