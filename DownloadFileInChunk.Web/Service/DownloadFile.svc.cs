using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace DownloadFileInChunk.Web.Service
{    
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DownloadFile : IDownloadFile
    {

        #region IDownloadFile Members

        /// <summary>
        /// Method used to Download File at client in Chunks to Improve performance
        /// Bussiness Logic: 1. Check FileType , based on FileType  get actual file path 
        ///                  2. Read file according to offset provided and return that bytes
        /// </summary>
        /// <param name="FileName">String</param>
        /// <param name="FileType">String</param>
        /// <param name="Offset">Int64</param>
        /// <param name="BufferSize">Int32</param>
        /// <returns></returns>
        public byte[] DownloadChunk(String DocUrl, Int64 Offset, Int32 BufferSize)
        {
            String FilePath = HttpContext.Current.Server.MapPath(DocUrl);
            if (!System.IO.File.Exists(FilePath))
                return null;
            Int64 FileSize = new FileInfo(FilePath).Length;
            //// if the requested Offset is larger than the file, quit.
            if (Offset > FileSize)
            {
                //SecurityService.logger.Fatal("Invalid Download Offset - " + String.Format("The file size is {0}, received request for offset {1}", FileSize, Offset));
                return null;
            }
            // open the file to return the requested chunk as a byte[]
            byte[] TmpBuffer;
            int BytesRead;
            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.Seek(Offset, SeekOrigin.Begin);	// this is relevent during a retry. otherwise, it just seeks to the start
                    TmpBuffer = new byte[BufferSize];
                    BytesRead = fs.Read(TmpBuffer, 0, BufferSize);	// read the first chunk in the buffer (which is re-used for every chunk)
                }
                if (BytesRead != BufferSize)
                {
                    // the last chunk will almost certainly not fill the buffer, so it must be trimmed before returning
                    byte[] TrimmedBuffer = new byte[BytesRead];
                    Array.Copy(TmpBuffer, TrimmedBuffer, BytesRead);
                    return TrimmedBuffer;
                }
                else
                    return TmpBuffer;
            }
            catch (Exception ex)
            {                
                return null;
            }
        }
        

        #endregion
    }
}
