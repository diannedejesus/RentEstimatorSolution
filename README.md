# RentEstimatorSolution

This is a desktop application for creating rent estimations for HCV participates. It produces a standard form provided as an example from the previous HCV guidebook.

## Use

Once downloaded and executed you will need to provide the payment standard for the desired area and utility allowance. These can be update through the the menu. You can opcionally provide an image and footer text for the document.

Once the information is updated, in the main window you input the voucher size, the families anual income, the amount of dependants and whether the family is handicapped or elderly. The you click on the create pdf button and it will use the provide information to estimate the eligible rent range and fill in the appropiate utility allowance for the family.

## Rent Calculation Class

This class is used to calculate the estimated rent a participant will be eligible for. These values are based on the number and calculations for 2023. Some of these values are set to be changed in 2024.

When the class is initialized for use it sets the path for FMR (payment standard) and utility allowance files. These are json files that contain the base values for these data and can be modified in the application.

All variables have default values.
(int) **Minimun rent**: 50
(int) **voucher size**: 0
(int) **dependants**: 0
(decimal) **annual income**: 0
(bool) **elderly or handicap**: false

**payment standard** is a dictionary of int, int
**utility allowance** is a list of custom model (utilities model)
**utilities model** is observableobject class that stores the values of bedroom, electricity, water, sewer, fridge, cooking, and microwave. All are int and more utilities values are used in programs but were not implemented in this application.

The constructor verifies if the files for the payment standard and utility allowance are present. If so the data is extracted for use. If now then an eeror is thrown.

Several methods are available:

(decimal) **Adjusted Annual Income**: Calculate the annual adjusted income. Uses the supplied (either directly or set through the class) elderlyOrDisabled (boolean), dependants (decimal), and annulincome (decimal) to calculate it. Alternatively if no values is supplied it will use the defaults set in the class which are set to 0 and false.

(decimal) **TTP Determination**: Calculates the tenant total pay. If supplied (either directly or set through the class), it will use annualIncome (decimal), adjustedAnnualIncome (decimal) and miniminRent (decimal) to do the calculation. If no minimun rent is sent it will use the default of 50. If no values are sent then it will use the defualts of 0 and 50 (min Rent).

(int) **Get FMR**: Fectches the correct payment standard for the supplied voucher size (int). If no voucher size is supplied (either directly or set through the class) it will use the default of 0.

(int) **Total Utilities**: Adds all the utilites for the supplied voucher size (int).

(int) **Get Total Utilities**: Adds the selected utilites for the selected voucher size (int), utitities are determined by a boolean.

(int) **Get Utility Amount**: Returns the selected utility (string: water, electricity, fridge, microwave, sewer, cooking) for the select voucher size (int).

(decimal) **MaxSubsidy**: Calculates the max subsidy. If the payment standard (decimal) and TTP (decimal) is supplied (directly) those are used if not it will call GetFMR() and TTPDetermination() and use the values set through the class or the default values.

(decimal) **Forty percent adjusted**: Calculates the 40% of the monthly adjusted income. By calling AdjustedAnnualIncome() which will use the supplied data (set through the class from the main window or the defaults) to do the calculation.

(decimal) **Gross rent**: Calculates the gross rent by calling FortypercentAdjusted() and maxSubsidy() with the supplied values (set through the class from the main window or the defaults) for the calculation.

(decimal) **Lowest rent**: Calculates the lowest rent. It calls TotalUtilities(\_voucherSize), GrossRent() and GetFMR() with the supplied values (set through the class from the main window or the defaults).

(int) **Round off**: Rounds of a decimal value using the common calculation used in banking.

## PdfTemplateBuilder Class
