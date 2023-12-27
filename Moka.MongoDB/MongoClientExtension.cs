using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Moka.MongoDB
{
    public static class MongoClientExtansion
    {
        public enum Order
        {
            Asc,
            Desc
        }

        public static string CreateIndex<T>(this IMongoCollection<T> collection, Order order, bool unique, params string[] fields)
        {
            var indexKeys = new List<IndexKeysDefinition<T>>();
            for (int i = 0; i < fields.Length; i++)
            {
                IndexKeysDefinition<T> indexKey;
                switch (order)
                {
                    //case Order.Asc:
                    //    break;
                    case Order.Desc:
                        indexKey = Builders<T>.IndexKeys.Descending(fields[i]);
                        indexKeys.Add(indexKey);
                        break;
                    default:
                        indexKey = Builders<T>.IndexKeys.Ascending(fields[i]);
                        indexKeys.Add(indexKey);
                        break;
                }
            }
            IndexKeysDefinition<T> combine = Builders<T>.IndexKeys.Combine(indexKeys);

            var createIndexOptions = new CreateIndexOptions { Unique = unique };

            var createIndexModel = new CreateIndexModel<T>(combine, createIndexOptions);
            return collection.Indexes.CreateOne(createIndexModel);
        }

        public static string CreateTextIndex<T>(this IMongoCollection<T> collection, params string[] fields)
        {
            var indexKeysDefinitionList = new List<IndexKeysDefinition<T>>();
            for (int i = 0; i < fields.Length; i++)
            {
                IndexKeysDefinition<T> item = Builders<T>.IndexKeys.Text(fields[i]);
                indexKeysDefinitionList.Add(item);
            }
            IndexKeysDefinition<T> combine = Builders<T>.IndexKeys.Combine(indexKeysDefinitionList);
            CreateIndexModel<T> createIndexModel = new CreateIndexModel<T>(combine);
            return collection.Indexes.CreateOne(createIndexModel);
        }

        public static bool TryInsert<T>(this IMongoCollection<T> collection, T document)
        {
            try
            {
                collection.InsertOne(document);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static long TryInsert<T>(this IMongoCollection<T> collection, List<T> documents)
        {
            if (documents.Count == 0) return 0;
            long total = Convert.ToInt64(documents.Count);
            try
            {
                var options = new InsertManyOptions() { IsOrdered = false };
                collection.InsertMany(documents, options);
            }
            catch (MongoBulkWriteException mongoBulkWriteException)
            {
                total -= mongoBulkWriteException.WriteErrors.Count;
            }
            return total;
        }
    }
}
