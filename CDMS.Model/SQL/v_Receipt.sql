ALTER VIEW [dbo].[v_Receipt]
AS
SELECT  a.AccountMonth,
		a.CustomerID					    AS CompanyID,
		'¦¬´Ú'								AS DealItem ,			
		ISNULL(SUM(CheckAmount),0) + ISNULL(SUM(CashAmount),0)         
											AS ReceiptAmount,	
		ISNULL(SUM(DiscountAmount),0)       AS SalesDiscount,
		ISNULL(SUM(ReturnAmount),0)         AS SalesDeduct
FROM [dbo].[Receipt] a (NOLOCK) 
GROUP BY a.AccountMonth,a.CustomerID
GO