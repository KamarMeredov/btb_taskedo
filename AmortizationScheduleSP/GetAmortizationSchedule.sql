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
	
	WITH AmortizationSchedule AS (
    SELECT 
        1 AS [PaymentNumber],
        @TotalAmount AS [BeginningBalance],
        @TotalAmount * @InterestRate AS [Interest],
        @MonthlyPayment - (@TotalAmount * @InterestRate) AS [Principal],
		@MonthlyPayment AS [PaymentAmount],
        CONVERT(DECIMAL(19, 2), @TotalAmount - (@MonthlyPayment - (@TotalAmount * @InterestRate))) AS [EndingBalance]
    UNION ALL
    SELECT 
        [PaymentNumber] + 1,
        [EndingBalance],
		[EndingBalance] * @InterestRate,
        @MonthlyPayment - ([EndingBalance] * @InterestRate),
		@MonthlyPayment,
        CONVERT(DECIMAL(19, 2), [EndingBalance] - (@MonthlyPayment - ([EndingBalance] * @InterestRate)))
    FROM 
        [AmortizationSchedule]
    WHERE 
        [PaymentNumber] < 12
	),
	
	
	LastPaymentData as (
	SELECT
		[PaymentNumber] AS [PaymentNumber],
		[EndingBalance] AS [EndingBalance],
		CONVERT(DECIMAL(19,10), @RecycledInterestRate * [EndingBalance] / (1 - POWER(1 + @RecycledInterestRate, -@RecycledLoanTerm))) AS [MonthlyPayment]
    FROM 
        [AmortizationSchedule]
    WHERE 
        [PaymentNumber] = 12
	),

	RecycledAmortizationSchedule AS (
	SELECT 
        [LastPaymentData].[PaymentNumber] + 1 AS [PaymentNumber],
        [LastPaymentData].[EndingBalance] AS [BeginningBalance],
		[LastPaymentData].[EndingBalance] * @RecycledInterestRate AS [Interest],
        [LastPaymentData].[MonthlyPayment] - ([LastPaymentData].[EndingBalance] * @RecycledInterestRate) AS [Principal],
		[LastPaymentData].[MonthlyPayment] AS [PaymentAmount],
        CONVERT(DECIMAL(19, 2), [LastPaymentData].[EndingBalance] - ([LastPaymentData].[MonthlyPayment] - ([LastPaymentData].[EndingBalance] * @RecycledInterestRate))) AS [EndingBalance]
    FROM
        [LastPaymentData]
	UNION ALL
	SELECT 
        [RecycledAmortizationSchedule].[PaymentNumber] + 1,
        [RecycledAmortizationSchedule].[EndingBalance] AS [BeginningBalance],
		[RecycledAmortizationSchedule].[EndingBalance] * @RecycledInterestRate AS [Interest],
        [LastPaymentData].[MonthlyPayment] - ([RecycledAmortizationSchedule].[EndingBalance] * @RecycledInterestRate) AS [Principal],
		[LastPaymentData].[MonthlyPayment] AS [PaymentAmount],
        CONVERT(DECIMAL(19, 2), [RecycledAmortizationSchedule].[EndingBalance] - ([LastPaymentData].[MonthlyPayment] - ([RecycledAmortizationSchedule].[EndingBalance] * @RecycledInterestRate))) AS [EndingBalance]
    FROM
		[RecycledAmortizationSchedule]
	CROSS JOIN
		[LastPaymentData]
	WHERE [RecycledAmortizationSchedule].[PaymentNumber] < 12 + @RecycledLoanTerm
	)

	SELECT
		[AmortizationSchedule].[PaymentNumber],
		[AmortizationSchedule].[BeginningBalance],
		CONVERT(DECIMAL(19,2), [AmortizationSchedule].[Interest]) AS [Interest],
		CONVERT(DECIMAL(19,2), [AmortizationSchedule].[Principal]) AS [Principal],
		CONVERT(DECIMAL(19,2), [AmortizationSchedule].[PaymentAmount]) AS [MonthlyPayment],
		[AmortizationSchedule].[EndingBalance]
	FROM [AmortizationSchedule]
	UNION ALL
	SELECT 
		[RecycledAmortizationSchedule].[PaymentNumber],
		[RecycledAmortizationSchedule].[BeginningBalance],
		CONVERT(DECIMAL(19,2), [RecycledAmortizationSchedule].[Interest]) AS [Interest],
		CONVERT(DECIMAL(19,2), [RecycledAmortizationSchedule].[Principal]) AS [Principal],
		CONVERT(DECIMAL(19,2), [RecycledAmortizationSchedule].[PaymentAmount]) AS [MonthlyPayment],
		[RecycledAmortizationSchedule].[EndingBalance]
	FROM [RecycledAmortizationSchedule]
END