﻿using System;
using DevExpress.Xpo;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraGrid.Views.Grid;
using WinFormsApplication.Forms;
using XpoTutorial;

namespace WinFormsApplication {
    public partial class OrdersListForm : RibbonForm {
        public OrdersListForm() {
            InitializeComponent();
        }
        private void OrdersGridView_RowClick(object sender, RowClickEventArgs e) {
            if(e.Clicks == 2) {
                e.Handled = true;
                int orderID = (int)OrdersGridView.GetRowCellValue(e.RowHandle, colOid);
                ShowEditForm(orderID);
            }
        }

        private void ShowEditForm(int? orderID) {
            var form = new EditOrderForm(orderID);
            form.FormClosed += EditFormClosed;
            var documentManager = DocumentManager.FromControl(MdiParent);
            if (documentManager != null) {
                documentManager.View.AddDocument(form);
            } else {
                try {
                    form.ShowDialog();
                } finally {
                    form.Dispose();
                }
            }
        }

        private void EditFormClosed(object sender, EventArgs e) {
            var form = (EditOrderForm)sender;
            form.FormClosed -= EditFormClosed;
            if (form.OrderID.HasValue) {
                Reload();
                OrdersGridView.FocusedRowHandle = OrdersGridView.LocateByValue("Oid", form.OrderID.Value,
                    rowHandle => OrdersGridView.FocusedRowHandle = (int)rowHandle);
            }
        }
        
        private void Reload() {
            OrdersInstantFeedbackView.Refresh();
        }

        private void BtnNew_ItemClick(object sender, ItemClickEventArgs e) {
            ShowEditForm(null);
        }

        private void BtnDelete_ItemClick(object sender, ItemClickEventArgs e) {
            using(Session session = new Session()) {
                object orderId = OrdersGridView.GetFocusedRowCellValue(colOid);
                Order order = session.GetObjectByKey<Order>(orderId);
                session.Delete(order);
            }
            Reload();
        }

        private void OrdersInstantFeedbackView_ResolveSession(object sender, ResolveSessionEventArgs e) {
            e.Session = new Session();
        }

        private void OrdersInstantFeedbackView_DismissSession(object sender, ResolveSessionEventArgs e) {
            e.Session.Session.Dispose();
        }
    }
}
