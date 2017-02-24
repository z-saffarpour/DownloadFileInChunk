#region using

using System;
using System.Xml;
using System.Web;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Web.Services.Protocols;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

#endregion 

namespace DownloadFileInChunk.Web.Service
{
    [ServiceContract]
    interface IDownloadFile
    {
        [OperationContract]
        byte[] DownloadChunk(String Docurl, Int64 Offset, Int32 BufferSize);
    }
}
