using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnisciPdf.BusinessLogic;

namespace UnisciPdf.ViewModels
{
    public class OptionPopupViewModel : ViewAware
    {
        private PdfCompressionOptions pdfCompressionOptions;

        public OptionPopupViewModel(PdfCompressionOptions pdfCompressionOptions)
        {
            this.pdfCompressionOptions = pdfCompressionOptions;
        }

        public bool CompressionEnabled => pdfCompressionOptions.CompressionEnabled;
        public bool ColorCompressionEnabled => pdfCompressionOptions.CompressionEnabled & pdfCompressionOptions.DownsampleColorImages;

        public bool DownsampleColorImages
        {
            get { return pdfCompressionOptions.DownsampleColorImages; }
            set
            {
                if (pdfCompressionOptions.DownsampleColorImages != value)
                {
                    pdfCompressionOptions.DownsampleColorImages = value;
                    NotifyOfPropertyChange(() => this.DownsampleColorImages);
                    NotifyOfPropertyChange(() => this.ColorCompressionEnabled);
                }
            }
        }


        public int ColorImageResolution
        {
            get { return pdfCompressionOptions.ColorImageResolution; }
            set
            {
                if (pdfCompressionOptions.ColorImageResolution != value)
                {
                    pdfCompressionOptions.ColorImageResolution = value;
                    NotifyOfPropertyChange(() => this.ColorImageResolution);
                }
            }
        }

        public double ColorImageDownsampleThreshold
        {
            get { return pdfCompressionOptions.ColorImageDownsampleThreshold; }
            set
            {
                if (pdfCompressionOptions.ColorImageDownsampleThreshold != value)
                {
                    pdfCompressionOptions.ColorImageDownsampleThreshold = value;
                    NotifyOfPropertyChange(() => this.ColorImageDownsampleThreshold);
                }
            }
        }

        public bool GrayCompressionEnabled => pdfCompressionOptions.CompressionEnabled & pdfCompressionOptions.DownsampleGrayImages;
        public bool DownsampleGrayImages
        {
            get { return pdfCompressionOptions.DownsampleGrayImages; }
            set
            {
                if (pdfCompressionOptions.DownsampleGrayImages != value)
                {
                    pdfCompressionOptions.DownsampleGrayImages = value;
                    NotifyOfPropertyChange(() => this.DownsampleGrayImages);
                    NotifyOfPropertyChange(() => this.GrayCompressionEnabled);
                }
            }
        }

        public int GrayImageResolution
        {
            get { return pdfCompressionOptions.GrayImageResolution; }
            set
            {
                if (pdfCompressionOptions.GrayImageResolution != value)
                {
                    pdfCompressionOptions.GrayImageResolution = value;
                    NotifyOfPropertyChange(() => this.GrayImageResolution);
                }
            }
        }

        public double GrayImageDownsampleThreshold
        {
            get { return pdfCompressionOptions.GrayImageDownsampleThreshold; }
            set
            {
                if (pdfCompressionOptions.GrayImageDownsampleThreshold != value)
                {
                    pdfCompressionOptions.GrayImageDownsampleThreshold = value;
                    NotifyOfPropertyChange(() => this.GrayImageDownsampleThreshold);
                }
            }
        }

        public bool MonoCompressionEnabled => pdfCompressionOptions.CompressionEnabled & pdfCompressionOptions.DownsampleMonoImages;
        public bool DownsampleMonoImages
        {
            get { return pdfCompressionOptions.DownsampleMonoImages; }
            set
            {
                if (pdfCompressionOptions.DownsampleMonoImages != value)
                {
                    pdfCompressionOptions.DownsampleMonoImages = value;
                    NotifyOfPropertyChange(() => this.DownsampleMonoImages);
                    NotifyOfPropertyChange(() => this.MonoCompressionEnabled);
                }
            }
        }

        public int MonoImageResolution
        {
            get { return pdfCompressionOptions.MonoImageResolution; }
            set
            {
                if (pdfCompressionOptions.MonoImageResolution != value)
                {
                    pdfCompressionOptions.MonoImageResolution = value;
                    NotifyOfPropertyChange(() => this.MonoImageResolution);
                }
            }
        }

        public double MonoImageDownsampleThreshold
        {
            get { return pdfCompressionOptions.MonoImageDownsampleThreshold; }
            set
            {
                if (pdfCompressionOptions.MonoImageDownsampleThreshold != value)
                {
                    pdfCompressionOptions.MonoImageDownsampleThreshold = value;
                    NotifyOfPropertyChange(() => this.MonoImageDownsampleThreshold);
                }
            }
        }
    }
}

