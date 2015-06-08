using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnisciPdf.BusinessLogic;
using UnisciPdf.Model;

namespace UnisciPdf.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IHaveDisplayName
    {

        #region Variables
        FileIdentificationService fileIdentificationService = new FileIdentificationService();
        PdfService pdfService = new PdfService();

        #endregion

        #region Properties
        public string DisplayName
        {
            get
            {
                return Properties.Resources.AppTitle;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private bool shellIsBusy = false;
        public bool ShellIsBusy
        {
            get { return shellIsBusy; }
            set
            {
                if (shellIsBusy != value)
                {
                    shellIsBusy = value;
                    NotifyOfPropertyChange(() => this.ShellIsBusy);
                }
            }
        }

        private ObservableCollection<FileAndOrder> fileListVar = new ObservableCollection<FileAndOrder>();

        public ObservableCollection<FileAndOrder> FileList
        {
            get { return fileListVar; }
            set
            {
                fileListVar = value;
                NotifyOfPropertyChange(() => this.FileList);
                NotifyOfPropertyChange(() => this.CanCreateFile);
                NotifyOfPropertyChange(() => this.CanReset);
            }
        }


        private string destinationFileName = null;
        public string DestinationFileName
        {
            get { return destinationFileName; }
            set
            {
                if (destinationFileName != value)
                {
                    destinationFileName = value;
                    NotifyOfPropertyChange(() => this.DestinationFileName);
                    NotifyOfPropertyChange(() => this.CanCreateFile);
                    NotifyOfPropertyChange(() => this.CanReset);
                }
            }
        }

        private string destinationFilePath = null;
        public string DestinationFilePath
        {
            get { return destinationFilePath; }
            set
            {
                if (destinationFilePath != value)
                {
                    destinationFilePath = value;
                    NotifyOfPropertyChange(() => this.DestinationFilePath);
                    NotifyOfPropertyChange(() => this.CanCreateFile);
                    NotifyOfPropertyChange(() => this.CanReset);
                }
            }
        }

        #endregion

        #region Actions

        public void Reset()
        {


        }

        public bool CanReset
        {
            get { return true; }
        }

        public void CreateFile()
        {
            List<string> fileListOrdered = FileList.OrderBy(d => d.Number).Select(d => d.FileFullPath).ToList();
            pdfService.MergePdf(fileListOrdered, Path.Combine(DestinationFilePath, DestinationFileName), null);
        }

        public bool CanCreateFile
        {
            get
            {
                if (!FileList.Any())
                    return false;

                if (string.IsNullOrWhiteSpace(DestinationFilePath))
                    return false;

                if (string.IsNullOrWhiteSpace(DestinationFileName))
                    return false;

                return true;
            }

        }
        #endregion


        #region Drag&Drop

        public void DragOver(DragEventArgs dropInfo)
        {
            var dropPossible = dropInfo.Data != null && ((DataObject)dropInfo.Data).ContainsFileDropList();
            if (dropPossible)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(DragEventArgs dropInfo)
        {
            ShellIsBusy = true;
            if (dropInfo.Data is DataObject && ((DataObject)dropInfo.Data).ContainsFileDropList())
            {
                ObservableCollection<FileAndOrder> list = new ObservableCollection<FileAndOrder>();
                foreach (string filePath in ((DataObject)dropInfo.Data).GetFileDropList())
                {
                    var attributes = File.GetAttributes(filePath);
                    bool isDir = (attributes & FileAttributes.Directory) == FileAttributes.Directory;

                    if (isDir)
                    {
                        foreach (var item in Directory.EnumerateFiles(filePath))
                        {
                            ProcessDroppedFile(list, item);
                        }
                    }
                    else
                        ProcessDroppedFile(list, filePath);
                }

                this.FileList = new ObservableCollection<FileAndOrder>(list.OrderBy(l => l.Number));

                if (FileList.Any())
                {
                    string dir = Path.GetDirectoryName(FileList.FirstOrDefault().FileFullPath);
                    if (FileList.All(d => Path.GetDirectoryName(d.FileFullPath) == dir))
                        this.DestinationFilePath = dir;
                }
            }
            ShellIsBusy = false;
        }

        private void ProcessDroppedFile(ObservableCollection<FileAndOrder> list, string filePath)
        {
            fileIdentificationService.CheckFileAndAdd(filePath, list);
        }

        #endregion
    }
}
