/***************************************************************************					
 ***名稱:	v_ReceivableDetail
 ***目的:	應收付單證明細
 ***備註:	
 ***************************************************************************/ 
ALTER VIEW [dbo].[v_ReceivableDetail]
AS

SELECT ISNULL(ROW_NUMBER ( ) OVER (ORDER BY a.ProductID),0) AS SeqNo,
	   a.*,
	   b.ProductName
FROM 
(
SELECT  SalesID AS VoucherID, ProductID, Qty, Price, Amount
FROM SalesDetail
UNION
SELECT PurchaseID AS VoucherID, ProductID, Qty, Price, Amount
FROM PurchaseDetail
UNION
SELECT InvoiceID AS VoucherID, ProductID, Qty, Price, Amount
FROM SalesInvoiceDetail
UNION
SELECT InvoiceID AS VoucherID, ProductID, Qty, Price, Amount
FROM PurchaseInvoiceDetail
)a LEFT JOIN Product b 
ON a.ProductID = b.ProductID 

Go

