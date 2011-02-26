﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using EzPos.Model;
using EzPos.Properties;
using EzPos.Utility;

namespace EzPos.GUIs.Forms
{
    public class PrintBarCode
    {
        private static readonly PrintDocument PrintDocument = new PrintDocument();
        private static List<BarCode> _barCodeList = new List<BarCode>();
        private static int _counter;
        private static StringFormat _strFormat;
        private static string _printType;

        public static void InializePrinting(List<BarCode> barCodeList, string printType)
        {
            _barCodeList = barCodeList;
            _printType = printType;
            var printPreviewDialog = 
                new PrintPreviewDialog
                {
                    WindowState = FormWindowState.Maximized,
                    FormBorderStyle = FormBorderStyle.None,
                    UseAntiAlias = true,
                    Document = PrintDocument
                };

            if (AppContext.Counter != null)
                PrintDocument.PrinterSettings.PrinterName = AppContext.Counter.BarCodePrinter;

            PrintDocument.BeginPrint += PrintDocBeginPrint;
            if (Resources.ConstBarCodeTemplate5.Equals(_printType))
            {
                PrintDocument.PrintPage += BarcodeSample5_PrintPage;
                printPreviewDialog.ShowDialog();
                PrintDocument.PrintPage -= BarcodeSample5_PrintPage;
            }
            else if (Resources.ConstBarCodeTemplate4.Equals(_printType))
            {
                PrintDocument.PrintPage += BarcodeSample4_PrintPage;
                printPreviewDialog.ShowDialog();
                PrintDocument.PrintPage -= BarcodeSample4_PrintPage;
            }
            else if (Resources.ConstBarCodeTemplate3.Equals(_printType))
            {
                PrintDocument.PrintPage += BarcodeSample3_PrintPage;
                printPreviewDialog.ShowDialog();
                PrintDocument.PrintPage -= BarcodeSample3_PrintPage;
            }
            else if (Resources.ConstBarCodeTemplate2.Equals(_printType))
            {
                PrintDocument.PrintPage += BarcodeSample2_PrintPage;
                printPreviewDialog.ShowDialog();
                PrintDocument.PrintPage -= BarcodeSample2_PrintPage;
            }            
            else if (Resources.ConstBarCodeTemplate1.Equals(_printType))
            {
                PrintDocument.PrintPage += BarcodeSample1_PrintPage;
                printPreviewDialog.ShowDialog();
                PrintDocument.PrintPage -= BarcodeSample1_PrintPage;
            }
            PrintDocument.BeginPrint -= PrintDocBeginPrint;
        }

        private static void BarcodeSample1_PrintPage(object sender, PrintPageEventArgs e)
        {
            var posY = 25;
            int rowIndex = 0, colIndex = 0;

            var fontBarCode = new Font("Free 3 of 9 Extended", 35, FontStyle.Regular);
            var solidBrush = new SolidBrush(Color.Black);
            var recHeight = (e.MarginBounds.Top + e.MarginBounds.Bottom) / 6;

            var leftMargin = e.MarginBounds.Left;
            var rightMargin = e.MarginBounds.Right;
            var medianPaper = e.MarginBounds.Width / 2;

            var posX = leftMargin - 50;
            while (_counter <= _barCodeList.Count - 1)
            {
                if (rowIndex == 6)
                {
                    e.HasMorePages = true;
                    return;
                }

                var barCode = _barCodeList[_counter];
                var printStr = "*" + barCode.BarCodeValue + "*";
                var txtWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Width, 0).ToString());
                var txtPosY = 
                    5 + Int32.Parse(Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Height, 0).ToString()) / 2;

                posX += medianPaper * colIndex;

                var pen = new Pen(solidBrush, 0.1f);
                var rectangle = 
                    colIndex < 1 ? 
                    new Rectangle(
                        posX,
                        posY,
                        medianPaper + 50,
                        recHeight - 20) : 
                    new Rectangle(
                        medianPaper + 100,
                        posY,
                        rightMargin - medianPaper - 50,
                        recHeight - 20);

                pen.Color = Color.White;
                e.Graphics.DrawRectangle(pen, rectangle);

                e.Graphics.DrawString(
                    printStr,
                    fontBarCode,
                    solidBrush,
                    rectangle.Left + ((rectangle.Width - txtWidth) / 2),
                    20 + posY + txtPosY,
                    _strFormat);

                var fontDisplayName = new Font("Arial", 12, FontStyle.Bold);
                printStr = barCode.BarCodeValue;
                txtWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontDisplayName).Width, 0).ToString());
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    rectangle.Left + ((rectangle.Width - txtWidth) / 2),
                    posY + txtPosY + 60,
                    _strFormat);

                fontDisplayName = new Font("Arial", 15, FontStyle.Bold);
                printStr = barCode.DisplayStr;
                txtWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontDisplayName).Width, 0).ToString());
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    rectangle.Left + ((rectangle.Width - txtWidth) / 2),
                    posY + txtPosY + 95,
                    _strFormat);

                printStr = barCode.UnitPrice;
                txtWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontDisplayName).Width, 0).ToString());
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    rectangle.Left + ((rectangle.Width - txtWidth) / 2),
                    posY + txtPosY + 120,
                    _strFormat);

                if (colIndex < 1)
                    colIndex++;
                else
                {
                    colIndex = 0;
                    posX = leftMargin - 50;
                    rowIndex++;
                    posY += (recHeight - 20);
                }

                _counter++;
            }
            e.HasMorePages = false;
        }

        private static void BarcodeSample2_PrintPage(object sender, PrintPageEventArgs e)
        {
            const string originStr = "MADE IN KOREA";
            const string prefixProductCode = "CO-765K-";
            var dateStr = 
                StringHelper.Right("000" + DateTime.Now.Day, 3) + 
                StringHelper.Right("000" + DateTime.Now.Month, 3) + 
                StringHelper.Right("000" + DateTime.Now.Year, 3);

            int rowIndex = 0, colIndex = 0;

            var fontBarCode = new Font("Free 3 of 9 Extended", 38, FontStyle.Regular);
            var solidBrush = new SolidBrush(Color.Black);

            var leftMargin = e.MarginBounds.Left;
            var topMargin = e.MarginBounds.Top;
            var recWidth = (e.MarginBounds.Left + e.MarginBounds.Right) / 3;
            var recHeight = (e.MarginBounds.Top + e.MarginBounds.Bottom) / 11;

            var posX = leftMargin - 70;
            var posY = topMargin - 80;
            var extraY = 0;
            var extraX = 0;
            while (_counter <= _barCodeList.Count - 1)
            {
                if (rowIndex == 12)
                {
                    e.HasMorePages = true;
                    return;
                }

                var barCode = _barCodeList[_counter];
                var printStr = "*" + barCode.BarCodeValue + "*";
                var barcodeTextWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Width, 0).ToString());

                //Origin
                var fontAscii = new Font("Arial", 10, FontStyle.Bold);
                printStr = originStr;
                e.Graphics.DrawString(
                    printStr,
                    fontAscii,
                    solidBrush,
                    posX + (recWidth * colIndex) + extraX - 10,
                    posY + (recHeight * rowIndex) + extraY,
                    _strFormat);

                //Date
                fontAscii = new Font("Arial", 8, FontStyle.Regular);
                printStr = dateStr;
                var displayTxtWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontAscii).Width, 0).ToString());

                e.Graphics.DrawString(
                    printStr,
                    fontAscii,
                    solidBrush,
                    posX + (recWidth * colIndex) + (recWidth - displayTxtWidth) - 60 + extraX,
                    posY + (recHeight * rowIndex) + extraY,
                    _strFormat);

                //Product code
                printStr = "*" + barCode.BarCodeValue + "*";
                e.Graphics.DrawString(
                    printStr,
                    fontBarCode,
                    solidBrush,
                    (posX + (recWidth * colIndex)) + ((recWidth - barcodeTextWidth) / 2) - 20 + extraX - 10,
                    posY + (recHeight * rowIndex) + 32 + extraY,
                    _strFormat);

                //Unit price
                fontAscii = new Font("Arial", 15, FontStyle.Bold);
                printStr = barCode.UnitPrice;
                e.Graphics.DrawString(
                    printStr,
                    fontAscii,
                    solidBrush,
                    posX + (recWidth * colIndex) + extraX - 10,
                    posY + (recHeight * rowIndex) + 64 + extraY,
                    _strFormat);

                //Product code with prefix
                fontAscii = new Font("Arial", 8, FontStyle.Regular);
                printStr = prefixProductCode + barCode.BarCodeValue;
                displayTxtWidth = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontAscii).Width, 0).ToString());

                e.Graphics.DrawString(
                    printStr,
                    fontAscii,
                    solidBrush,
                    posX + (recWidth * colIndex) + (recWidth - displayTxtWidth) - 60 + extraX,
                    posY + (recHeight * rowIndex) + 64 + extraY,
                    _strFormat);

                if (colIndex < 2)
                {
                    colIndex++;
                    extraX -= 10;
                }
                else
                {
                    colIndex = 0;
                    posX = leftMargin - 70;
                    rowIndex++;
                    extraY -= 15;
                    extraX = 0;
                }

                _counter++;
            }
            e.HasMorePages = false;
        }

        private static void BarcodeSample3_PrintPage(object sender, PrintPageEventArgs e)
        {
            int posX = 12, posY = 30;
            int rowIndex = 0, colIndex = 0;

            var fontBarCode = new Font("Free 3 of 9 Extended", 24, FontStyle.Regular);
            var solidBrush = new SolidBrush(Color.Black);
            var recWidth = (e.MarginBounds.Left + e.MarginBounds.Right) / 5;
            var recHeight = (e.MarginBounds.Top + e.MarginBounds.Bottom) / 8;

            while (_counter <= _barCodeList.Count - 1)
            {
                var fontDisplayName = new Font("Arial", 8, FontStyle.Bold);
                if (rowIndex == 8)
                {
                    e.HasMorePages = true;
                    return;
                }

                var barCode = _barCodeList[_counter];
                var printStr = "*" + barCode.BarCodeValue + "*";
                var widthBarCode = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Width, 0).ToString());
                var txtPosY = 5 + Int32.Parse(
                                      Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Height, 0).ToString()) /
                                  2;

                printStr =
                    StringHelper.Right("000" + DateTime.Today.Day, 3) +
                    StringHelper.Right("000" + DateTime.Today.Month, 3) +
                    StringHelper.Right("000" + DateTime.Today.Year, 3);

                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    ((2 * posX) + recWidth - widthBarCode) / 2 + 2,
                    posY,
                    _strFormat);

                float xValue = ((2 * posX) + recWidth - widthBarCode) / 2 + 0;
                printStr = "*" + barCode.BarCodeValue + "*";
                e.Graphics.DrawString(
                    printStr,
                    fontBarCode,
                    solidBrush,
                    xValue,
                    5 + posY + txtPosY,
                    _strFormat);

                fontDisplayName = new Font("Arial", 10, FontStyle.Bold);
                printStr = barCode.BarCodeValue;
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    ((2 * posX) + recWidth - widthBarCode) / 2 + 5,
                    posY + txtPosY + 30,
                    _strFormat);

                fontDisplayName = new Font("Arial", 13, FontStyle.Bold);
                widthBarCode = ((2 * posX) + recWidth - widthBarCode) / 2 + widthBarCode;
                //printStr = "$" + barCode.DisplayStr;
                printStr = barCode.UnitPrice;
                printStr = printStr.Replace(" ", "");
                printStr = printStr.Replace(",", "");
                var widthTxt = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontDisplayName).Width, 0).ToString());
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    widthBarCode - widthTxt - 5,
                    posY + txtPosY + 30,
                    _strFormat);

                if (colIndex < 4)
                {
                    colIndex++;
                    posX += recWidth - 4;
                }
                else
                {
                    colIndex = 0;
                    posX = 12;
                    rowIndex++;
                    if (rowIndex == 2)
                        posY -= 10;
                    if (rowIndex == 5)
                        posY -= 10;

                    posY += recHeight;
                }

                _counter++;
            }
            e.HasMorePages = false;
        }

        private static void BarcodeSample4_PrintPage(object sender, PrintPageEventArgs e)
        {
            int posX = 20, posY = 20;
            int rowIndex = 0, colIndex = 0;

            var fontBarCode = new Font("Free 3 of 9 Extended", 24, FontStyle.Regular);
            var solidBrush = new SolidBrush(Color.Black);
            var recWidth = (e.MarginBounds.Left + e.MarginBounds.Right) / 5;
            var recHeight = (e.MarginBounds.Top + e.MarginBounds.Bottom) / 12;
            var extraX = -10;

            while (_counter <= _barCodeList.Count - 1)
            {
                var fontDisplayName = new Font("Arial", 8, FontStyle.Regular);
                if (rowIndex == 12)
                {
                    e.HasMorePages = true;
                    return;
                }

                var barCode = _barCodeList[_counter];
                var printStr = "*" + barCode.BarCodeValue + "*";
                var widthBarCode = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Width, 0).ToString());
                var txtPosY = 
                    5 + 
                    Int32.Parse(Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Height, 0).ToString()) / 2;

                printStr =
                    StringHelper.Right("000" + DateTime.Today.Day, 3) +
                    StringHelper.Right("000" + DateTime.Today.Month, 3) +
                    StringHelper.Right("000" + DateTime.Today.Year, 3);

                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    ((2 * posX) + recWidth - widthBarCode) / 2 + 2 + extraX,
                    posY,
                    _strFormat);

                float xValue = ((2 * posX) + recWidth - widthBarCode) / 2 + 0;
                printStr = "*" + barCode.BarCodeValue + "*";
                e.Graphics.DrawString(
                    printStr,
                    fontBarCode,
                    solidBrush,
                    xValue + extraX,
                    5 + posY + txtPosY,
                    _strFormat);

                fontDisplayName = new Font("Arial", 8, FontStyle.Regular);
                printStr = barCode.BarCodeValue;
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    ((2 * posX) + recWidth - widthBarCode) / 2 + 2 + extraX,
                    posY + txtPosY + 30,
                    _strFormat);

                fontDisplayName = new Font("Arial", 8, FontStyle.Regular);
                widthBarCode = ((2 * posX) + recWidth - widthBarCode) / 2 + widthBarCode;
                //printStr = "$" + barCode.DisplayStr;
                printStr = barCode.UnitPrice;
                printStr = printStr.Replace(" ", "");
                printStr = printStr.Replace(",", "");
                var widthTxt = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontDisplayName).Width, 0).ToString());
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    widthBarCode - widthTxt - 8 + extraX,
                    posY + txtPosY + 30,
                    _strFormat);

                if (colIndex < 4)
                {
                    colIndex++;
                    posX += recWidth - 4;
                    extraX -= 5;
                }
                else
                {
                    extraX = -10;
                    colIndex = 0;
                    posX = 20;
                    rowIndex++;
                    if (rowIndex == 2)
                        posY -= 10;
                    if (rowIndex == 5)
                        posY -= 10;

                    posY += recHeight;
                }

                _counter++;
            }
            e.HasMorePages = false;
        }

        private static void BarcodeSample5_PrintPage(object sender, PrintPageEventArgs e)
        {
            int posX = 20, posY = 20;
            int rowIndex = 0, colIndex = 0;

            var fontBarCode = new Font("Free 3 of 9 Extended", 24, FontStyle.Regular);
            var solidBrush = new SolidBrush(Color.Black);
            var recWidth = (e.MarginBounds.Left + e.MarginBounds.Right) / 5;
            var recHeight = (e.MarginBounds.Top + e.MarginBounds.Bottom) / 12;
            var extraX = -10;

            while (_counter <= _barCodeList.Count - 1)
            {
                if (rowIndex == 12)
                {
                    e.HasMorePages = true;
                    return;
                }

                var barCode = _barCodeList[_counter];
                var printStr = "*" + barCode.BarCodeValue + "*";
                var widthBarCode = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Width, 0).ToString());
                var txtPosY =
                    5 +
                    Int32.Parse(Math.Round(e.Graphics.MeasureString(printStr, fontBarCode).Height, 0).ToString()) / 2;

                var fontDisplayName = new Font("Arial", 6, FontStyle.Regular);
                //printStr =
                //    StringHelper.Right("000" + DateTime.Today.Day, 3) +
                //    StringHelper.Right("000" + DateTime.Today.Month, 3) +
                //    StringHelper.Right("000" + DateTime.Today.Year, 3);
                printStr = barCode.Description;
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    ((2 * posX) + recWidth - widthBarCode) / 2 + 2 + extraX,
                    posY,
                    _strFormat);

                float xValue = ((2 * posX) + recWidth - widthBarCode) / 2 + 0;
                printStr = "*" + barCode.BarCodeValue + "*";
                e.Graphics.DrawString(
                    printStr,
                    fontBarCode,
                    solidBrush,
                    xValue + extraX,
                    5 + posY + txtPosY,
                    _strFormat);

                fontDisplayName = new Font("Arial", 8, FontStyle.Regular);
                printStr = barCode.BarCodeValue;
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    ((2 * posX) + recWidth - widthBarCode) / 2 + 2 + extraX,
                    posY + txtPosY + 30,
                    _strFormat);

                fontDisplayName = new Font("Arial", 8, FontStyle.Regular);
                widthBarCode = ((2 * posX) + recWidth - widthBarCode) / 2 + widthBarCode;
                //printStr = "$" + barCode.DisplayStr;
                printStr = barCode.UnitPrice;
                printStr = printStr.Replace(" ", "");
                printStr = printStr.Replace(",", "");
                var widthTxt = Int32.Parse(
                    Math.Round(e.Graphics.MeasureString(printStr, fontDisplayName).Width, 0).ToString());
                e.Graphics.DrawString(
                    printStr,
                    fontDisplayName,
                    solidBrush,
                    widthBarCode - widthTxt - 8 + extraX,
                    posY + txtPosY + 30,
                    _strFormat);

                if (colIndex < 4)
                {
                    colIndex++;
                    posX += recWidth - 4;
                    extraX -= 5;
                }
                else
                {
                    extraX = -10;
                    colIndex = 0;
                    posX = 20;
                    rowIndex++;
                    if (rowIndex == 2)
                        posY -= 10;
                    if (rowIndex == 5)
                        posY -= 10;

                    posY += recHeight;
                }

                _counter++;
            }
            e.HasMorePages = false;
        }

        private static void PrintDocBeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                _strFormat = 
                    new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                _counter = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, 
                    Resources.MsgCaptionError, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}