using MongoDB.Bson;

namespace Moka.MongoDB
{
    public static class BsonExtension
    {
        public static BsonValue GetBsonValue(this BsonDocument bsonDocument, params string[] keys)
        {
            if (bsonDocument == null) return null;
            BsonDocument result = bsonDocument;
            BsonValue bsonValue = null;
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                if (result.Contains(key))
                {
                    bsonValue = result[key];
                }
                else
                {
                    return null;
                }

                if (i != keys.Length - 1)
                {
                    result = bsonValue.AsBsonDocument;
                }
            }
            return bsonValue;
        }

        public static string GetStringValue(this BsonDocument bsonDocument, params string[] keys)
        {
            BsonValue value = GetBsonValue(bsonDocument, keys);
            if (value == null)
            {
                return null;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
