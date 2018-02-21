ALTER VIEW [dbo].[v_ReceivableSummary]
AS
SELECT  a.CompanyID						AS CompanyID,
		a.ShortName						AS CompanyName,
		a.AccountMonth					AS AccountMonth,
		ISNULL(a.SalesAmount,0 )		AS SalesAmount,
		ISNULL(a.SalesTax,0 )			AS SalesTax,
		ISNULL(a.PurchaseAmount,0 )		AS PurchaseAmount,
		ISNULL(a.PurchaseTax,0 )		AS PurchaseTax,
		ISNULL(a.ReceiptAmount,0 )		AS ReceiptAmount,
		ISNULL(a.SalesDeduct,0 )		AS SalesDeduct,
		ISNULL(a.SalesDiscount,0 )		AS SalesDiscount,
		ISNULL(a.PaymentAmount,0 )		AS PaymentAmount,
		ISNULL(a.PurchaseDeduct,0 )		AS PurchaseDeduct,
		ISNULL(a.PurchaseDiscount,0 )	AS PurchaseDiscount,
		ISNULL(a.Remaining,0 )			AS Remaining,
		a.AccountDate					AS AccountDate,	
		ISNULL(b.Remaining,0)			AS LastBalance,
		ISNULL(a.Remaining,0) + ISNULL(b.Remaining, 0) 
										AS Balance		
FROM v_ReceivableSummaryBase a (NOLOCK) LEFT JOIN v_ReceivableSummaryBase b (NOLOCK)
ON a.CompanyID = b.CompanyID AND DATEADD(MONTH,-1, a.AccountDate) = b.AccountDate
GO
