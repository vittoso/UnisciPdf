using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UnisciPdf.BusinessLogic;
using UnisciPdf.Model;

namespace UnisciPdf.ViewModels
{

    public class ShellViewModel : Screen, IHaveDisplayName
    {

        public ShellViewModel()
        {
            this.fileIdentificationService = new FileIdentificationService();
            this.pdfService = new PdfService();
            this.PdfCompressionOptions = pdfService.GetDefaultCompressionOptions();

            this.DestinationFileName = ".pdf";

            if (Execute.InDesignMode)
                LoadDesignData();
        }

        private void LoadDesignData()
        {

        }


        #region Variables
        FileIdentificationService fileIdentificationService;
        PdfService pdfService;

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



        private bool compressionEnabled = false;
        public bool CompressionEnabled
        {
            get { return compressionEnabled; }
            set
            {
                if (compressionEnabled != value)
                {
                    compressionEnabled = value;
                    NotifyOfPropertyChange(() => this.CompressionEnabled);
                }
            }
        }



        private string imageQuality = null;
        public string ImageQuality
        {
            get { return imageQuality; }
            set
            {
                if (imageQuality != value)
                {
                    imageQuality = value;
                    NotifyOfPropertyChange(() => this.ImageQuality);
                }
            }
        }



        private PdfCompressionOptions pdfCompressionOptions = null;
        public PdfCompressionOptions PdfCompressionOptions
        {
            get { return pdfCompressionOptions; }
            set
            {
                if (pdfCompressionOptions != value)
                {
                    pdfCompressionOptions = value;
                    NotifyOfPropertyChange(() => this.PdfCompressionOptions);
                }
            }
        }

        #endregion

        #region Actions

        public void Reset()
        {
            FileList.Clear();
            DestinationFileName = ".pdf";
        }

        public bool CanReset
        {
            get { return true; }
        }

        public void CreateFile()
        {
            if (Path.GetExtension(DestinationFileName) != ".pdf")
                DestinationFileName = string.Concat(DestinationFileName, ".pdf");


            this.ShellIsBusy = true;
            List<string> fileListOrdered = FileList.OrderBy(d => d.Number).Select(d => d.FileFullPath).ToList();

            Task.Run(() =>
            {
                pdfService.MergePdf(fileListOrdered, Path.Combine(DestinationFilePath, DestinationFileName), this.PdfCompressionOptions);
            }).ContinueWith((task) =>
            {
                this.ShellIsBusy = false;
            });
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

                string filenameWithoutExt = Path.GetFileNameWithoutExtension(DestinationFileName);

                if (filenameWithoutExt == null || filenameWithoutExt.Length == 0)
                    return false;

                if (filenameWithoutExt.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    return false;

                string extension = Path.GetExtension(DestinationFileName);

                if (extension.Length > 0 && extension.ToLower() != ".pdf")
                    return false;

                return true;
            }

        }

        public void ShowOptions()
        {
            WindowManager wm = new WindowManager();
            dynamic settings = new ExpandoObject();
            //    settings.Width = 200;
            //   settings.PopupAnimation = PopupAnimation.Fade;
            //settings.Placement = PlacementMode.Absolute;
            //settings.HorizontalOffset = SystemParameters.FullPrimaryScreenWidth / 2 - 100;
            //settings.VerticalOffset = SystemParameters.FullPrimaryScreenHeight / 2 - 50;
            wm.ShowDialog(new OptionPopupViewModel(this.PdfCompressionOptions), null, settings);
        }
        #endregion

        public void FileNumberEditEnding(DataGridCellEditEndingEventArgs e, object dataContext)
        {

            if (e.EditAction == DataGridEditAction.Commit && e.EditingElement != null)
            {
                FileAndOrder value = e.EditingElement.DataContext as FileAndOrder;

                if (value != null)
                {
                    // var list = new List<FileAndOrder>(this.FileList.OrderBy(f => f.Number));

                    //// collection does not start from 1
                    //if (!list.Any(f => f.Number == 1))
                    //{
                    //    list = list.OrderBy(f => f.Number).ToList();
                    //    int number = 1;
                    //    foreach (var item in list)
                    //    {
                    //        if (item != value)
                    //            item.Number = number++;

                    //        if (number == value.Number)
                    //            ++number;
                    //    }
                    //    list = list.OrderBy(f => f.Number).ToList();
                    //}


                    //  this.FileList = new ObservableCollection<FileAndOrder>(list);

                }

            }

        }


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

                if (FileList.Any() && string.IsNullOrEmpty(this.DestinationFilePath))
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

        protected override void OnDeactivate(bool close)
        {
            Properties.Settings.Default.DestinationFilePath = this.DestinationFilePath;
            Properties.Settings.Default.CompressionEnabled = this.CompressionEnabled;
            Properties.Settings.Default.CutMargins = this.PdfCompressionOptions.CutMargins;
            Properties.Settings.Default.CutMargins_Top_Points = this.PdfCompressionOptions.CutMarginsTopPoints;
            Properties.Settings.Default.CutMargins_Bottom_Points = this.PdfCompressionOptions.CutMarginsBottomPoints;
            Properties.Settings.Default.CutMargins_Left_Points = this.PdfCompressionOptions.CutMarginsLeftPoints;
            Properties.Settings.Default.CutMargins_Right_Points = this.PdfCompressionOptions.CutMarginsRightPoints;
            Properties.Settings.Default.Save();
            base.OnDeactivate(close);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.DestinationFilePath = Properties.Settings.Default.DestinationFilePath;
            this.CompressionEnabled = Properties.Settings.Default.CompressionEnabled;
            this.PdfCompressionOptions.CutMargins = Properties.Settings.Default.CutMargins;
            this.PdfCompressionOptions.CutMarginsTopPoints = Properties.Settings.Default.CutMargins_Top_Points;
            this.PdfCompressionOptions.CutMarginsBottomPoints = Properties.Settings.Default.CutMargins_Bottom_Points;
            this.PdfCompressionOptions.CutMarginsLeftPoints = Properties.Settings.Default.CutMargins_Left_Points;
            this.PdfCompressionOptions.CutMarginsRightPoints = Properties.Settings.Default.CutMargins_Right_Points;
        }
    }
}
