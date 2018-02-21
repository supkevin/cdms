ALTER VIEW [dbo].[v_Receivable]
AS
SELECT  a.AccountMonth,
		a.CompanyID,
		b.ShortName							 AS CompanyName,
		a.DealItem							 AS DealItem ,
		ISNULL(SUM(SalesAmount),0)			 AS SalesAmount,
		ISNULL(SUM(SalesTax),0)              AS SalesTax,	
		ISNULL(SUM(PurchaseAmount),0)        AS PurchaseAmount,
		ISNULL(SUM(PurchaseTax),0)           AS PurchaseTax,
		CONVERT(SMALLDATETIME, '1801' + '01', 12 ) AS AccountDate		
FROM [dbo].[Receivable] a (NOLOCK) LEFT JOIN Company b (NOLOCK)
ON a.CompanyID = b.CompanyID
GROUP BY a.AccountMonth,a.CompanyID,b.ShortName,a.DealItem
GO
