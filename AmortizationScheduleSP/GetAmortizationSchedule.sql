CREATE PROCEDURE [dbo].[GetAmortizationSchedule]
(
	@TotalAmount DECIMAL(19,2) = 36000,
	@Interest DECIMAL(19,2) = 8,
	@LoanTerm INT = 36,
	@RecycledInterest DECIMAL(19,2) = 4.5,
	@RecycledLoanTerm INT = 48
)
AS
BEGIN
	DECLARE @InterestRate DECIMAL(19,10);
	DECLARE @MonthlyPayment DECIMAL(19,2);
	DECLARE @RecycledInterestRate DECIMAL(19,10);

	SET @InterestRate = @Interest / 12 / 100;
	SET @RecycledInterestRate = @RecycledInterest / 12 / 100;
	SET @MonthlyPayment = (@InterestRate * @TotalAmount) / (1 - POWER(1 + @InterestRate, -@LoanTerm));
	
	-- AmortizationSchedule table for first 12 payments with 8% interst
	WITH AmortizationSchedule AS (
	SELECT 
		1 AS [PaymentNumber],
		@TotalAmount AS [LoanAmount],
		@TotalAmount * @InterestRate AS [Interest],
		@MonthlyPayment - (@TotalAmount * @InterestRate) AS [Principal],
		@MonthlyPayment AS [PaymentAmount],
		CONVERT(DECIMAL(19, 2), @TotalAmount - (@MonthlyPayment - (@TotalAmount * @InterestRate))) AS [EndingLoanBalance]
	UNION ALL
	SELECT 
		[PaymentNumber] + 1,
		[EndingLoanBalance],
		[EndingLoanBalance] * @InterestRate,
		@MonthlyPayment - ([EndingLoanBalance] * @InterestRate),
		@MonthlyPayment,
		CONVERT(DECIMAL(19, 2), [EndingLoanBalance] - (@MonthlyPayment - ([EndingLoanBalance] * @InterestRate)))
	FROM 
		[AmortizationSchedule]
	WHERE 
		[PaymentNumber] < 12
	),
		
	-- Save last payment information into temporary table for the next 48 payments with 4.5% interest
	LastPaymentData AS (
	SELECT
		[PaymentNumber],
		[EndingLoanBalance],
		CONVERT(DECIMAL(19,10), @RecycledInterestRate * [EndingLoanBalance] / (1 - POWER(1 + @RecycledInterestRate, -@RecycledLoanTerm))) AS [MonthlyPayment]
	FROM 
		[AmortizationSchedule]
	WHERE 
		[PaymentNumber] = 12
	),

	-- Amortization table for next 48 payments with 4.5% interest
	RecycledAmortizationSchedule AS (
	SELECT 
		[PaymentNumber] + 1 AS [PaymentNumber],
		[EndingLoanBalance] AS [LoanAmount],
		[EndingLoanBalance] * @RecycledInterestRate AS [Interest],
		[MonthlyPayment] - ([EndingLoanBalance] * @RecycledInterestRate) AS [Principal],
		[MonthlyPayment] AS [PaymentAmount],
		CONVERT(DECIMAL(19, 2), [EndingLoanBalance] - ([MonthlyPayment] - ([EndingLoanBalance] * @RecycledInterestRate))) AS [EndingLoanBalance]
	FROM
		[LastPaymentData]
	UNION ALL
	SELECT 
		[RAS].[PaymentNumber] + 1,
		[RAS].[EndingLoanBalance] AS [LoanAmount],
		[RAS].[EndingLoanBalance] * @RecycledInterestRate AS [Interest],
		[PaymentAmount] - ([RAS].[EndingLoanBalance] * @RecycledInterestRate) AS [Principal],
		[PaymentAmount] AS [PaymentAmount],
		CONVERT(DECIMAL(19, 2), [RAS].[EndingLoanBalance] - ([PaymentAmount] - ([RAS].[EndingLoanBalance] * @RecycledInterestRate))) AS [EndingLoanBalance]
	FROM
		[RecycledAmortizationSchedule] AS [RAS]
	WHERE 
		[RAS].[PaymentNumber] < 12 + @RecycledLoanTerm
	)

	-- Merge two amortization tables with 8% and 4.5% payments to get final result table
	SELECT
		[PaymentNumber],
		[LoanAmount],
		CONVERT(DECIMAL(19,2), [Interest]) AS [Interest],
		CONVERT(DECIMAL(19,2), [Principal]) AS [Principal],
		CONVERT(DECIMAL(19,2), [PaymentAmount]) AS [PaymentAmount],
		[EndingLoanBalance]
	FROM 
		[AmortizationSchedule]
	UNION ALL
	SELECT 
		[PaymentNumber],
		[LoanAmount],
		CONVERT(DECIMAL(19,2), [Interest]),
		CONVERT(DECIMAL(19,2), [Principal]),
		CONVERT(DECIMAL(19,2), [PaymentAmount]),
		[EndingLoanBalance]
	FROM 
		[RecycledAmortizationSchedule]
END