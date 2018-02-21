CREATE VIEW [dbo].[v_Payment]
AS
SELECT  a.AccountMonth,
		a.SupplierID					    AS CompanyID,
		'¥I´Ú'								AS DealItem ,			
		ISNULL(SUM(CheckAmount),0) + ISNULL(SUM(CashAmount),0)         
											AS PaymentAmount,	
		ISNULL(SUM(DiscountAmount),0)       AS PurchaseDiscount,
		ISNULL(SUM(ReturnAmount),0)         AS PurchaseDeduct
FROM [dbo].[Payment] a (NOLOCK) 
GROUP BY a.AccountMonth,a.SupplierID
GO