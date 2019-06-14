namespace JCE.Sage.Providers
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JCE.Crm.Plugins.Entities;
    #endregion

    /// <summary>
    /// The interface implmented by Order Providers
    /// </summary>
    public interface IOrderProvider
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>Returns an order record.</returns>
        SalesOrder GetOrder(Guid orderId);

        /// <summary>
        /// Gets the order detail.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>A list of order details</returns>
        List<SalesOrderDetail> GetOrderDetail(Guid orderId);

        /// <summary>
        /// Gets the type of the order detail by product type.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="productType">The product type.</param>
        /// <returns>A list of order details of the specific product type.</returns>
        List<SalesOrderDetail> GetOrderDetail(Guid orderId, ProductType productType);

        /// <summary>
        /// Gets the product extras.
        /// </summary>
        /// <param name="salesOrderId">The sales order id.</param>
        /// <returns>A list of product extras (i.e. related order details) for the product.</returns>
        List<SalesOrderDetail> GetProductExtras(Guid salesOrderId);

        /// <summary>
        /// Calculates the amount paid.
        /// </summary>
        /// <param name="salesOrderId">The sales order id.</param>
        void CalculateAmountPaid(Guid salesOrderId);
    }
}
