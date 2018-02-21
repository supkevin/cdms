SELECT ReceiptID, CustomerID, AccountMonth, BankAccountID, ReceiptDate, DueDate, CheckNum, CheckAmount, CashAmount, DiscountAmount, ReturnAmount, Remarks, Activate
FROM [dbo].[Receipt]
ORDER BY ReceiptID

SELECT PaymentID, SupplierID, BankAccountID, AccountMonth, PayDate, DueDate, CheckNum, CheckAmount, CashAmount, DiscountAmount, ReturnAmount, Remarks, Activate
FROM [dbo].[Payment]
ORDER BY PaymentID

SELECT SeqNo, SourceID, DealDate, ExpiryDate, Summary, DebitAmount, CreditAmount, CompanyID, CheckNum, BankID, AccountID, BankAccountID, CheckStatus, Activate
FROM [dbo].[BankDeposit]
ORDER BY SeqNo

