using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public static class HeaderDoc
    {
        public static string FieldName(string fieldName)
        {
            Verify.IsNotEmpty(nameof(fieldName), fieldName);

            return $"{nameof(HeaderDoc<Tag>.Payload)}.{fieldName}";
        }
    }

    public class HeaderDoc<T> where T : class
    {
        public HeaderDoc()
        {
        }

        public HeaderDoc(T payload)
        {
            Verify.IsNotNull(nameof(payload), payload);

            Payload = payload;
            ETag = Payload.CalculateETag();
            UpdatedDate = DateTime.UtcNow;
        }

        //[BsonIgnoreIfNull]
        //public ObjectId? _id { get; set; }

        public string ETag { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.Int64)]
        public DateTime UpdatedDate { get; set; }

        public T Payload { get; set; }
    }
}
