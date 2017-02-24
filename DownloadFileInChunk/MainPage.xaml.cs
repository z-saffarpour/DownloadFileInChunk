using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DownloadFileInChunk.DownloadFile;
using System.IO;
using System.Windows.Browser;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;


namespace DownloadFileInChunk
{
    public partial class MainPage : UserControl
    {
        DownloadFileClient service = null;
        Int32 i32ChunkSize = 10 * 1024 * 1024;
        Int64 I64Offset = 0;
        string docName = String.Empty;
        bool IsFileToAppend = false;
        Int64 fileSize = 0;
        SaveFileDialog fileDialog = null;
        bool isFirstCall = true;
        Stream fileStream = null;

        public MainPage()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            string docPath = App.TargetFolder + "//" + "123.pdf";
            DownloadFile(docPath);
        }

        public void DownloadFile(string DocURL)
        {     
            // first you should get the file size from the server side in bytes.
            fileSize = 825344;
            service = new DownloadFileClient();
            isFirstCall = true;           
            service.InnerChannel.OperationTimeout = new TimeSpan(0, 30, 0);
            docName = DocURL;
            service.DownloadChunkAsync(DocURL, I64Offset, i32ChunkSize);
            service.DownloadChunkCompleted += new EventHandler<DownloadChunkCompletedEventArgs>(service_DownloadChunkCompleted);
                   
        }

        void service_DownloadChunkCompleted(object sender, DownloadChunkCompletedEventArgs e)
        {
            try
            {
                Byte[] byteArrayFile = (Byte[])e.Result;
                if (isFirstCall)
                {
                    MessageBox _oBox = MessageBox.Show("Download file in chunk", "Are you sure to download the file ?", MsgBoxButtons.YesNo, MsgBoxIcon.Error, OnDialogClosed);
                }
                isFirstCall = false;
                if (fileDialog != null)
                {
                    WriteIntoFile(fileDialog, byteArrayFile);
                    I64Offset = I64Offset + i32ChunkSize;
                    if (I64Offset < fileSize)
                    {
                        IsFileToAppend = true;                        
                        service.DownloadChunkAsync(docName, I64Offset, i32ChunkSize);
                    }
                    else
                    {
                        I64Offset = 0;
                        fileStream.Close();
                        System.Windows.MessageBox.Show("File downloaded succesfully.");
                    }
                }
                else
                    service.DownloadChunkAsync(docName, I64Offset, i32ChunkSize);
            }
            catch (Exception ex)
            {
                
            }
        }

        bool OnDialogClosed(object sender, DialogExit e)
        {         
            if (e == DialogExit.OK)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "All files (*.*)|*.* |PDF Files (*.pdf)|*.pdf";
                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult == true)
                {
                    fileDialog = dialog;
                    fileStream = (Stream)dialog.OpenFile();
                } 
            }
            else if (e == DialogExit.Cancel)
            {
               
            }
            return true;
        }
       
        /// <summary>
        /// Write bytes into file.
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="data"></param>
        public void WriteIntoFile(SaveFileDialog dialog, byte[] data)
        {
            try
            {
                if (fileStream != null)
                {
                    fileStream.Write(data, 0, data.Length);
                    //fileStream.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }       
    }
}
