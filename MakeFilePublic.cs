﻿using System.Collections.Generic;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;

namespace ConsoleApp12
{
    public static class MakeFilePublic
    {
        public static string MakePublic(string bucketName,string objectName )
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName);
            storageObject.Acl ??= new List<ObjectAccessControl>();
            storage.UpdateObject(storageObject, new UpdateObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });

            return storageObject.MediaLink;
        }
    }
}
