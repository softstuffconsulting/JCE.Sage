namespace JCE.Sage.Providers
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using JCE.Sage.Providers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Messages;
    using JCE.Crm.Plugins.Entities;
    #endregion

    /// <summary>
    /// The implementation of the Order Provider
    /// </summary>
    public class OrderProvider : CrmProviderBase, IOrderProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactProvider"/> class.
        /// </summary>
        /// <param name="crmConnector">The CRM connector.</param>
        public OrderProvider(ICrmConnector crmConnector)
            : base(crmConnector)
        {
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>An order record.</returns>
        public SalesOrder GetOrder(Guid orderId)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return context.SalesOrderSet.FirstOrDefault(s => s.Id == orderId);
            }
        }

        /// <summary>
        /// Gets the order detail.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>A list of order details.</returns>
        public List<SalesOrderDetail> GetOrderDetail(Guid orderId)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return context.SalesOrderDetailSet.Where(s => s.SalesOrderId == new EntityReference(SalesOrderDetail.EntityLogicalName, orderId)).ToList();
            }
        }

        /// <summary>
        /// Gets the type of the order detail by product type.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="productType">The product type.</param>
        /// <returns>A list of order details of the specific product type.</returns>
        public List<SalesOrderDetail> GetOrderDetail(Guid orderId, ProductType productType)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return
                    (from s in context.SalesOrderDetailSet
                     join p in context.ProductSet on s.ProductId equals new EntityReference(SalesOrderDetail.EntityLogicalName, p.Id)
                     where s.SalesOrderId == new EntityReference(SalesOrder.EntityLogicalName, orderId)
                     where p.ProductTypeCode.Value == (int)ProductType.Course
                     select s).ToList();
            }
        }

        /// <summary>
        /// Gets the product extras.
        /// </summary>
        /// <param name="salesOrderId">The sales order id.</param>
        /// <returns>A list of product extras (i.e. related order details) for the product.</returns>
        public List<SalesOrderDetail> GetProductExtras(Guid salesOrderId)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return (from s in context.SalesOrderDetailSet
                        join ss in context.lss_salesorderdetail_salesorderdetailSet on s.SalesOrderDetailId equals ss.salesorderdetailidTwo
                        where ss.salesorderdetailidOne == salesOrderId
                        select s).ToList();
            }
        }

        /// <summary>
        /// Calculates the amount paid.
        /// </summary>
        /// <param name="salesOrderId">The sales order id.</param>
        public void CalculateAmountPaid(Guid salesOrderId)
        {
            // Get all the payment records for the order and add them together
            using (var context = this.CrmConnector.CreateDataContext())
            {
                var payments = context.lss_paymentSet.Where(p => p.lss_orderid == new EntityReference(SalesOrder.EntityLogicalName, salesOrderId) 
                    && p.lss_paystatus.Value == (int)PaymentProvider.PaymentStatus.Successful
                    && p.statecode == lss_paymentState.Active).ToList();
                
                decimal totalAmount = 0;
                foreach (lss_payment payment in payments)
                {
                    totalAmount += payment.lss_amount == null ? 0 : payment.lss_amount.Value;
                }

                var salesOrder = context.SalesOrderSet.FirstOrDefault(s => s.Id == salesOrderId);
                salesOrder.lss_Deposit = new Money(totalAmount);
                context.UpdateObject(salesOrder);
                context.SaveChanges();
            }
        }
    }
}
