using Microsoft.Spark.Sql;
using System;

namespace MyKafkaSparkApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SparkSession spark = SparkSession
                .Builder().AppName("word_count_sample")
                .Config("spark.jars.packages", "org.apache.spark:spark-sql-kafka-0-10_2.12:3.0.1").Master("local[*]")
                .GetOrCreate();

            spark.SparkContext.SetLogLevel("ERROR");

            DataFrame dataFrame = spark.ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", "localhost:9092")
                .Option("subscribe", "venda_topic")
                .Option("startingOffsets", "earliest")
                .Load().SelectExpr("CAST(value AS STRING)");

            var query = dataFrame.WriteStream()
               .Format("console")
               .Start();

            query.AwaitTermination();

            dataFrame.Show();

            spark.Stop();
        }
    }
}
