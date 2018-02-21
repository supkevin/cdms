ALTER VIEW [dbo].[v_ReceivableSummaryBase]
AS
SELECT  a.CompanyID,	
		a.ShortName,
		b.AccountMonth,	
		ISNULL(b.SalesAmount,0)			AS	SalesAmount,
		ISNULL(b.SalesTax,0)			AS	SalesTax,
		ISNULL(b.PurchaseAmount,0)		AS	PurchaseAmount,
		ISNULL(b.PurchaseTax,0)			AS	PurchaseTax,
		ISNULL(c.ReceiptAmount,0)		AS	ReceiptAmount,
		ISNULL(c.SalesDeduct,0)			AS	SalesDeduct,
		ISNULL(c.SalesDiscount,0)		AS	SalesDiscount,	
		ISNULL(d.PaymentAmount,0)		AS	PaymentAmount,
		ISNULL(d.PurchaseDeduct,0)		AS	PurchaseDeduct,
		ISNULL(d.PurchaseDiscount,0)	AS  PurchaseDiscount,
		(ISNULL(b.SalesAmount,0) - ISNULL(b.SalesTax,0) 
			- ISNULL(b.PurchaseAmount, 0) + ISNULL(b.PurchaseTax,0) 
			+ ISNULL(c.ReceiptAmount, 0) - ISNULL(c.SalesDeduct,0) - ISNULL(c.SalesDiscount,0)
			- ISNULL(d.PaymentAmount, 0) + ISNULL(d.PurchaseDeduct,0) + ISNULL(d.PurchaseDiscount,0))			
			AS Remaining,
		CONVERT(SMALLDATETIME,	b.AccountMonth + '01', 12) AS AccountDate	
FROM Company a (NOLOCK) JOIN  [dbo].[v_Receivable] b (NOLOCK)
ON a.CompanyID = b.CompanyID
LEFT JOIN v_Receipt c (NOLOCK) 
ON a.CompanyID = c.CompanyID AND c.AccountMonth = b.AccountMonth
LEFT JOIN v_Payment d (NOLOCK) 
ON a.CompanyID = c.CompanyID AND d.AccountMonth = b.AccountMonth
GO
