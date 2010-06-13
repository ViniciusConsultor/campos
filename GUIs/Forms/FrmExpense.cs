﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using EzPos.Model;
using EzPos.Properties;
using EzPos.Service;
using EzPos.Service.Common;

namespace EzPos.GUIs.Forms
{
    public partial class FrmExpense : Form
    {
        private CommonService CommonService;
        private ExpenseService ExpenseService;
        private bool IsModified;

        public FrmExpense()
        {
            InitializeComponent();
        }

        public Expense Expense
        {
            get { return _Expense; }
            set { _Expense = value; }
        }

        private void SetModifydStatus(bool modifyStatus)
        {
            IsModified = modifyStatus;
            btnSave.Enabled = modifyStatus;
        }

        private void ModificationHandler(object sender, EventArgs e)
        {
            SetModifydStatus(true);
        }

        private void CmbCategoryEnter(object sender, EventArgs e)
        {
            cmbExpenseType.SelectedIndexChanged += ModificationHandler;
        }

        private void CmbCategoryLeave(object sender, EventArgs e)
        {
            cmbExpenseType.SelectedIndexChanged -= ModificationHandler;
            cmbExpenseType.TextChanged -= ModificationHandler;
        }

        private void FrmExpense_Load(object sender, EventArgs e)
        {
            if (ExpenseService == null)
                ExpenseService = ServiceFactory.GenerateServiceInstance().GenerateExpenseService();

            if (CommonService == null)
                CommonService = ServiceFactory.GenerateServiceInstance().GenerateCommonService();

            ThreadStart threadStart = UpdateControlContent;
            var thread = new Thread(threadStart);
            thread.Start();
        }

        private void SetExpenseInfo()
        {
            if (_Expense == null)
                return;

            try
            {
                cmbExpenseType.SelectedValue = _Expense.ExpenseTypeID;
                dtpExpenseDate.Value = (DateTime) _Expense.ExpenseDate;
                txtDescription.Text = _Expense.Description;
                txtExpenseAmountRiel.Text = _Expense.ExpenseAmountRiel.ToString("N");
                txtExpenseAmountInt.Text = _Expense.ExpenseAmountInt.ToString("N");
            }
            catch (Exception exception)
            {
                ExtendedMessageBox.UnknownErrorMessage(
                    Resources.MsgCaptionUnknownError,
                    exception.Message);
            }
        }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            try
            {
                if (cmbExpenseType.SelectedIndex == -1)
                {
                    const string briefMsg = "អំពីពត៌មាន";
                    var detailMsg = Resources.MsgInvalidData;
                    using (var frmMessageBox = new ExtendedMessageBox())
                    {
                        frmMessageBox.BriefMsgStr = briefMsg;
                        frmMessageBox.DetailMsgStr = detailMsg;
                        frmMessageBox.IsCanceledOnly = true;
                        frmMessageBox.ShowDialog(this);
                        return;
                    }
                }

                if (_Expense == null)
                    _Expense = new Expense();

                _Expense.ExpenseTypeID = int.Parse(cmbExpenseType.SelectedValue.ToString());
                _Expense.ExpenseTypeStr = cmbExpenseType.Text;
                _Expense.ExpenseDate = dtpExpenseDate.Value.Date;
                _Expense.Description = txtDescription.Text;
                _Expense.ExpenseAmountRiel = float.Parse(txtExpenseAmountRiel.Text);
                _Expense.ExpenseAmountInt = float.Parse(txtExpenseAmountInt.Text);
                _Expense.ExchangeRate = AppContext.ExchangeRate.ExchangeValue;

                ExpenseService.ExpenseManagement(
                    _Expense,
                    _Expense.ExpenseID != 0 ? Resources.OperationRequestUpdate : Resources.OperationRequestInsert);

                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                ExtendedMessageBox.UnknownErrorMessage(
                    Resources.MsgCaptionUnknownError,
                    exception.Message);
            }
        }

        private void UpdateControlContent()
        {
            if (ExpenseService == null)
                ExpenseService = ServiceFactory.GenerateServiceInstance().GenerateExpenseService();

            SafeCrossCallBackDelegate safeCrossCallBackDelegate = null;
            if (cmbExpenseType.InvokeRequired)
                safeCrossCallBackDelegate = UpdateControlContent;

            if (cmbExpenseType.InvokeRequired)
                Invoke(safeCrossCallBackDelegate);
            else
            {
                var searchCriteria = new List<string> {"ParameterTypeID IN (22)"};
                var objList = CommonService.GetAppParameters(searchCriteria);

                CommonService.PopAppParamExtendedCombobox(
                    ref cmbExpenseType, objList, int.Parse(Resources.AppParamExpense), true);

                SetExpenseInfo();
                SetModifydStatus(false);
            }
        }

        private void TxtDescriptionEnter(object sender, EventArgs e)
        {
            try
            {
                InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("km-KH"));
            }
            catch (Exception)
            {
                InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("ca"));
            }

            txtDescription.TextChanged += ModificationHandler;
        }

        private void TxtDescriptionLeave(object sender, EventArgs e)
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("en"));
            lblKeyboardLayout.Visible = false;
            txtDescription.TextChanged -= ModificationHandler;
        }

        private void TxtExpenseAmountRielEnter(object sender, EventArgs e)
        {
            txtExpenseAmountRiel.TextChanged += ModificationHandler;
        }

        private void TxtExpenseAmountRielLeave(object sender, EventArgs e)
        {
            txtExpenseAmountRiel.TextChanged -= ModificationHandler;
            try
            {
                txtExpenseAmountRiel.Text = float.Parse(txtExpenseAmountRiel.Text).ToString("N");
            }
            catch (Exception exception)
            {
                ExtendedMessageBox.UnknownErrorMessage(
                    Resources.MsgCaptionUnknownError,
                    exception.Message);
            }
        }

        private void TxtExpenseAmountIntEnter(object sender, EventArgs e)
        {
            txtExpenseAmountInt.TextChanged += ModificationHandler;
        }

        private void TxtExpenseAmountIntLeave(object sender, EventArgs e)
        {
            txtExpenseAmountInt.TextChanged -= ModificationHandler;
            try
            {
                txtExpenseAmountInt.Text = float.Parse(txtExpenseAmountInt.Text).ToString("N");
            }
            catch (Exception exception)
            {
                ExtendedMessageBox.UnknownErrorMessage(
                    Resources.MsgCaptionUnknownError,
                    exception.Message);
            }
        }

        private void FrmExpense_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((DialogResult == DialogResult.Cancel) && (IsModified))
            {
                const string briefMsg = "អំពីការបោះបង់";
                var detailMsg = Resources.MsgOperationRequestCancel;
                using (var frmMessageBox = new ExtendedMessageBox())
                {
                    frmMessageBox.BriefMsgStr = briefMsg;
                    frmMessageBox.DetailMsgStr = detailMsg;
                    if (frmMessageBox.ShowDialog(this) != DialogResult.OK)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            if (!IsModified)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
        }

        private void BtnSaveMouseEnter(object sender, EventArgs e)
        {
            btnSave.BackgroundImage = Resources.background_9;
        }

        private void BtnSaveMouseLeave(object sender, EventArgs e)
        {
            btnSave.BackgroundImage = Resources.background_2;
        }

        private void BtnCancelMouseEnter(object sender, EventArgs e)
        {
            btnCancel.BackgroundImage = Resources.background_9;
        }

        private void BtnCancelMouseLeave(object sender, EventArgs e)
        {
            btnCancel.BackgroundImage = Resources.background_2;
        }

        private void DtpExpenseDateEnter(object sender, EventArgs e)
        {
            dtpExpenseDate.ValueChanged += ModificationHandler;
        }

        private void DtpExpenseDateLeave(object sender, EventArgs e)
        {
            dtpExpenseDate.ValueChanged -= ModificationHandler;
        }

        #region Nested type: SafeCrossCallBackDelegate

        private delegate void SafeCrossCallBackDelegate();

        #endregion

        private void FrmExpense_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            if (InputLanguage.CurrentInputLanguage != null)
                lblKeyboardLayout.Text = InputLanguage.CurrentInputLanguage.Culture.TwoLetterISOLanguageName;
            lblKeyboardLayout.Visible = true;
        }
    }
}