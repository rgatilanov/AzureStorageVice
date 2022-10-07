//Ligas de referencia:
//https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-dotnet-get-started
//https://learn.microsoft.com/en-us/dotnet/api/azure.storage.blobs.blobcontainerclient?view=azure-dotnet
//https://www.claudiobernasconi.ch/2022/02/25/how-to-upload-a-file-to-azure-blob-storage-dotnet6/
//Aquí encontré parar cambiar el acceso a público de la url del blob
//https://stackoverflow.com/questions/61653422/download-blob-using-azure-blob-storage-client-library-v12-for-net

using Azure.Storage.Blobs;
using Azure.Storage;
using System.Net.Http.Headers;
using Azure.Storage.Blobs.Models;
using static System.Reflection.Metadata.BlobBuilder;
using System.Reflection.Metadata;
using System.ComponentModel;
using System.Web;
using Azure;

Console.WriteLine("Iniciando prueba....");
BlobServiceClient blobServiceClient = null;
BlobContainerClient blobContainerClient = null;
//BlobClient blobClient = new BlobClient("{my_connectionstring}", "{my_content}", "{path\blob}");


//Paso 1: se autentica a la cuenta de almacenamiento:

//Se puede realizar la autenticación a la cuenta de almacenamiento por nombre de cuenta y key
GetBlobServiceClientByAccountKey(ref blobServiceClient, "{account_name}","{account_key}");

//Se puede realizar la autenticación a la cuenta de almacenamiento por connectionstring
//GetBlobServiceClientByConnectionString(ref blobServiceClient, "{my_connectionstring}");

//Paso 2: se crea container
GetBlobContainer(ref blobContainerClient, "{content_name}", "{my_connectionstring}");

//Paso 3: Hacer la subida del archivo
string[] Files;

Files = System.IO.Directory.GetFiles("C:\\files", "*.pdf");
foreach (string fila in Files)
{
    UploadFile(blobContainerClient, File.ReadAllBytes(fila), "myFolder", "archivito");
}

Console.WriteLine("Uploaded");

//Paso 4: Obtener la uri del archivo blob
//var uriBlob = GetUriBlob(blobClient);

static void GetBlobServiceClientByAccountKey(ref BlobServiceClient blobServiceClient,
    string accountName, string accountKey)
{
    Azure.Storage.StorageSharedKeyCredential sharedKeyCredential =
        new StorageSharedKeyCredential(accountName, accountKey);

    string blobUri = "https://" + accountName + ".blob.core.windows.net";

    blobServiceClient = new BlobServiceClient
        (new Uri(blobUri), sharedKeyCredential);

}

static void GetBlobServiceClientByConnectionString(ref BlobServiceClient blobServiceClient, string connectionString)
{
    blobServiceClient = new BlobServiceClient(connectionString);

}

static void GetBlobContainer(ref BlobContainerClient blobContainerClient, string nameContainer, string connectionString)
{
    blobContainerClient = new BlobContainerClient(connectionString, nameContainer);
    blobContainerClient.CreateIfNotExists();

}

static void UploadFile(BlobContainerClient blobContainerClient, byte[] file, string folderName, string nameBlob)
{
    Guid id = Guid.NewGuid();
    //Con nombre aleatorio
    //string blobPath = string.Format("{0}/{1}_{2}.{3}", folderName, id.ToString(),nameBlob,"pdf");

    //Con nombre como viene el archivo
    string blobPath = string.Format("{0}/{1}.{2}", folderName, nameBlob, "pdf");
    using (var stream = new MemoryStream(file))
    {
        blobContainerClient.UploadBlob(blobPath, stream);
    }
}

static string GetUriBlob(BlobClient blobClient)
{
    var blobUrl = blobClient.Uri.AbsoluteUri;
    return blobUrl;
}



