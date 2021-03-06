﻿using Caliburn.Micro;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Samples;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnisciPdf.BusinessLogic
{
    public class PdfService
    {

        public void MergePdf(List<string> fullPathFiles, string destinationFilePath, PdfCompressionOptions options)
        {
            Rectangle r = options.PageSize;
            using (FileStream stream = new FileStream(destinationFilePath, FileMode.Create))
            using (Document doc = new Document())
            using (PdfWriter writer = PdfWriter.GetInstance(doc, stream))
            {
                doc.Open();
                PdfContentByte cb = writer.DirectContent;
                PdfReader reader = null;
                PdfImportedPage page = null;

                //fixed typo
                fullPathFiles.ForEach(file =>
                {
                    reader = new PdfReader(file);

                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        if (r != null)
                        {
                            doc.SetPageSize(r);
                        }

                        doc.NewPage();
                        page = writer.GetImportedPage(reader, i + 1);
                        //   cb.AddTemplate(page, r.Width, r.Height, false);


                        float widthFactor = doc.PageSize.Width / page.Width;
                        float heightFactor = doc.PageSize.Height / page.Height;
                        float factor = Math.Min(widthFactor, heightFactor);

                        float offsetX = (doc.PageSize.Width - (page.Width * widthFactor)) / 2;
                        float offsetY = (doc.PageSize.Height - (page.Height * heightFactor)) / 2;

                        if (options.CutMargins)
                        {
                            var widthCutMargins = doc.PageSize.Width < doc.PageSize.Height ? options.CutMarginsLeftPoints + options.CutMarginsRightPoints : options.CutMarginsTopPoints + options.CutMarginsBottomPoints;
                            var heightCutMargins = doc.PageSize.Width < doc.PageSize.Height ? options.CutMarginsTopPoints + options.CutMarginsBottomPoints : options.CutMarginsLeftPoints + options.CutMarginsRightPoints;

                            var widthDestinationMargins = doc.PageSize.Width < doc.PageSize.Height ? doc.LeftMargin + doc.RightMargin : doc.TopMargin + doc.BottomMargin;
                            var heightDestinationMargins = doc.PageSize.Width < doc.PageSize.Height ? doc.TopMargin + doc.BottomMargin : doc.LeftMargin + doc.RightMargin;

                            widthFactor = doc.PageSize.Width / (page.Width - widthCutMargins);
                            heightFactor = doc.PageSize.Height / (page.Height - heightCutMargins);
                            factor = Math.Min(widthFactor, heightFactor);

                            offsetX = (doc.PageSize.Width - (page.Width * widthFactor)) / 2;
                            offsetY = (doc.PageSize.Height - (page.Height * heightFactor)) / 2;
                        }


                        cb.AddTemplate(page, widthFactor, 0, 0, heightFactor, offsetX, offsetY);


                    }
                    writer.FreeReader(reader);
                    reader.Close();

                });

                doc.Close();

            }

            if (options.CompressionEnabled)
                CompressPDF(destinationFilePath, options);
        }

        public void CompressPDF(string destinationFilePath, PdfCompressionOptions options)
        {

            string tempFile = System.IO.Path.GetTempFileName();

            File.WriteAllBytes(tempFile, File.ReadAllBytes(destinationFilePath));

            try
            {
                string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string dll = System.IO.Path.Combine(path, @"gs\gsdll32.dll");
                Ghostscript.NET.GhostscriptVersionInfo version = new Ghostscript.NET.GhostscriptVersionInfo(new System.Version(9, 16, 0), dll, string.Empty, Ghostscript.NET.GhostscriptLicense.GPL);
                using (Ghostscript.NET.Processor.GhostscriptProcessor processor = new GhostscriptProcessor(version))
                {
                    // processor.Processing += new GhostscriptProcessorProcessingEventHandler(ghostscript_Processing);
                    processor.Process(CreateCompressionGhostScriptArgs(tempFile, destinationFilePath, options), new ConsoleStdIO(false, true, true));
                }
            }
            finally { File.Delete(tempFile); }
        }

        /*
        public void CompressPDF(string destinationFilePath, PdfCompressionOptions options)
        {

            string tempFile = System.IO.Path.GetTempFileName();

            File.WriteAllBytes(tempFile, File.ReadAllBytes(destinationFilePath));

            try
            {

                Process process = new Process();

                //   process.StartInfo.Arguments = string.Format(@"-o {0} -sDEVICE=pdfwrite   -dDownsampleColorImages=true   -dDownsampleGrayImages=true  -dDownsampleMonoImages=true -dColorImageResolution=200    -dGrayImageResolution=150   -dMonoImageResolution=150   -dColorImageDownsampleThreshold=1.0   -dGrayImageDownsampleThreshold=1.0   -dMonoImageDownsampleThreshold=1.0  {1}", destinationFilePath, tempFile);
                process.StartInfo.Arguments = CreateCompressionGhostScriptArgs(tempFile, destinationFilePath, options);
                process.StartInfo.FileName = @"gswin32c.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();


                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            Console.Write(e.Data);
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            Console.Write(e.Data);
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    if (process.HasExited == false) process.Kill();
                }
            }
            finally { File.Delete(tempFile); }
        }
        public string CreateCompressionGhostScriptArgs(string inputPath, string outputPath, PdfCompressionOptions options)
        {
            StringBuilder gsArgs = new StringBuilder(" ");

            gsArgs.Append(@"-o " + outputPath).Append(" ");
            gsArgs.Append(@"-sDEVICE=pdfwrite").Append(" ");

            if (options.DownsampleColorImages)
            {
                gsArgs.Append(@"-dDownsampleColorImages=true").Append(" ");
                gsArgs.Append(@"-dColorImageResolution=").Append(options.ColorImageResolution).Append(" ");
                gsArgs.Append(@"-dColorImageDownsampleThreshold=").Append(options.ColorImageDownsampleThreshold).Append(" ");
            }

            if (options.DownsampleGrayImages)
            {
                gsArgs.Append(@"-dDownsampleGrayImages=true").Append(" ");
                gsArgs.Append(@"-dGrayImageResolutionn=").Append(options.GrayImageResolution).Append(" ");
                gsArgs.Append(@"-dGrayImageDownsampleThreshold=").Append(options.GrayImageDownsampleThreshold).Append(" ");
            }

            if (options.DownsampleMonoImages)
            {
                gsArgs.Append(@"-dDownsampleMonoImages=true").Append(" ");
                gsArgs.Append(@"-dMonoImageResolution=").Append(options.MonoImageResolution).Append(" ");
                gsArgs.Append(@"-dMonoImageDownsampleThreshold=").Append(options.MonoImageDownsampleThreshold).Append(" ");
            }

            gsArgs.Append(@"-f " + inputPath);


            return gsArgs.ToString();
        }
        */

        public string[] CreateCompressionGhostScriptArgs(string inputPath, string outputPath, PdfCompressionOptions options)
        {
            List<string> gsArgs = new List<string>();

            gsArgs.Add("-empty"); // mandatory for some reason...
            gsArgs.Add(@"-o");
            gsArgs.Add(outputPath);
            gsArgs.Add(@"-sDEVICE=pdfwrite");

            if (options.DownsampleColorImages)
            {
                gsArgs.Add("-dDownsampleColorImages=true");
                gsArgs.Add($"-dColorImageResolution={options.ColorImageResolution}");
                gsArgs.Add($"-dColorImageDownsampleThreshold={options.ColorImageDownsampleThreshold}");
            }

            if (options.DownsampleGrayImages)
            {
                gsArgs.Add("-dDownsampleGrayImages=true");
                gsArgs.Add($"-dGrayImageResolution={options.GrayImageResolution}");
                gsArgs.Add($"-dGrayImageDownsampleThreshold={options.GrayImageDownsampleThreshold}");
            }

            if (options.DownsampleMonoImages)
            {
                gsArgs.Add("-dDownsampleMonoImages=true");
                gsArgs.Add($"-dMonoImageResolution={options.MonoImageResolution}");
                gsArgs.Add($"-dMonoImageDownsampleThreshold={options.MonoImageDownsampleThreshold}");
            }

            if (options.DetectDuplicateImages)
                gsArgs.Add("-dDetectDuplicateImages=true");

            // https://gist.github.com/lkraider/f0888da30bc352f9d167dfa4f4fc8213
            if (options.ForceConversionCMYKToRGB)
            {
                gsArgs.Add("-sProcessColorModel=DeviceRGB");
                gsArgs.Add("-sColorConversionStrategy=sRGB");
                gsArgs.Add("-sColorConversionStrategyForImages=sRGB");
                gsArgs.Add("-dConvertCMYKImagesToRGB=true");
            }

            gsArgs.Add(@"-f");
            gsArgs.Add(inputPath);


            return gsArgs.ToArray();
        }

        public PdfCompressionOptions GetDefaultCompressionOptions()
        {
            PdfCompressionOptions cmp = new PdfCompressionOptions
            {
                CompressionEnabled = true,
                PageSize = PageSize.A4,
                DownsampleColorImages = true,
                ColorImageResolution = 200,
                ColorImageDownsampleThreshold = 1.0,
                DownsampleGrayImages = true,
                GrayImageResolution = 150,
                GrayImageDownsampleThreshold = 1.0,
                DownsampleMonoImages = true,
                MonoImageResolution = 150,
                MonoImageDownsampleThreshold = 1.0,
                DetectDuplicateImages = false,
                ForceConversionCMYKToRGB = false
            };

            return cmp;
        }

        /* Not really woring with itextsharp :(
         * public void ReduceResolution(PdfReader reader, long quality, string destinationFilePath)
            {

                List<PdfName> list = new List<PdfName>();
                int n = reader.XrefSize;
                for (int i = 0; i < n; i++)
                {
                    PdfObject obj = reader.GetPdfObject(i);
                    if (obj == null || !obj.IsStream()) { continue; }

                    PdfDictionary dict = (PdfDictionary)PdfReader.GetPdfObject(obj);
                    PdfName subType = (PdfName)PdfReader.GetPdfObject(dict.Get(PdfName.SUBTYPE));
                    if (!PdfName.IMAGE.Equals(subType) && !PdfName.IMAGEMASK.Equals(subType)) { continue; }

                    PRStream stream = (PRStream)obj;
                    try
                    {
                        PdfImageObject image = new PdfImageObject(stream);
                        PdfName filter = (PdfName)image.Get(PdfName.FILTER);
                        list.Add(filter);
                        if (
                          PdfName.JBIG2DECODE.Equals(filter)
                          || PdfName.JPXDECODE.Equals(filter)
                          || PdfName.CCITTFAXDECODE.Equals(filter)
                          || PdfName.FLATEDECODE.Equals(filter)
                        ) continue;

                        System.Drawing.Image img = image.GetDrawingImage();
                        if (img == null) continue;



                        var ll = image.GetImageBytesType();
                        int width = img.Width;
                        int height = img.Height;
                        PdfObject bitsPerComponent = image.Get(PdfName.BITSPERCOMPONENT);
                        PdfObject colorSpace = image.Get(PdfName.COLORSPACE);
                        using (System.Drawing.Bitmap dotnetImg =
                           new System.Drawing.Bitmap(img))
                        {

                            dotnetImg.Save(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(destinationFilePath), i.ToString() + ".jpg"));

                            // set codec to jpeg type => jpeg index codec is "1"
                            System.Drawing.Imaging.ImageCodecInfo codec =
                            System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];
                            // set parameters for image quality
                            System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
                            eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                            using (MemoryStream msImg = new MemoryStream())
                            {
                                dotnetImg.Save(msImg, codec, eParams);
                                msImg.Position = 0;
                                stream.SetData(msImg.ToArray());
                                //  stream.SetData(msImg.ToArray(), false, PRStream.BEST_COMPRESSION);
                                stream.Put(PdfName.TYPE, PdfName.XOBJECT);
                                stream.Put(PdfName.SUBTYPE, subType);
                                stream.Put(PdfName.FILTER, filter);
                                // stream.Put(PdfName.FILTER, PdfName.DCTDECODE);
                                stream.Put(PdfName.WIDTH, new PdfNumber(width));
                                stream.Put(PdfName.HEIGHT, new PdfNumber(height));
                                stream.Put(PdfName.BITSPERCOMPONENT, bitsPerComponent);
                                //  stream.Put(PdfName.BITSPERCOMPONENT, bitsPerComponent);
                                stream.Put(PdfName.COLORSPACE, colorSpace);
                                // stream.Put(PdfName.COLORSPACE, colorSpace);
                            }
                        }
                    }
                    catch
                    {
                        // throw;
                        // iText[Sharp] can't handle all image types...
                        continue;
                    }
                    finally
                    {
                        // may or may not help      
                        reader.RemoveUnusedObjects();
                    }
                }
            }  */
    }

    [ImplementPropertyChanged]
    public class PdfCompressionOptions
    {

        public bool CompressionEnabled { get; set; }

        public Rectangle PageSize { get; set; }

        public bool CutMargins { get; set; }

        public int CutMarginsTopPoints { get; set; }
        public int CutMarginsBottomPoints { get; set; }
        public int CutMarginsLeftPoints { get; set; }
        public int CutMarginsRightPoints { get; set; }


        public bool DownsampleColorImages { get; set; }
        public bool DownsampleGrayImages { get; set; }
        public bool DownsampleMonoImages { get; set; }

        public int ColorImageResolution { get; set; }
        public int GrayImageResolution { get; set; }
        public int MonoImageResolution { get; set; }

        public double ColorImageDownsampleThreshold { get; set; }
        public double GrayImageDownsampleThreshold { get; set; }
        public double MonoImageDownsampleThreshold { get; set; }


        public bool DetectDuplicateImages { get; set; }
        public bool ForceConversionCMYKToRGB { get; set; }
    }
}
