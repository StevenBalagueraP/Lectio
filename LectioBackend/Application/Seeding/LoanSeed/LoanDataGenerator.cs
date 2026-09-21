using Lectio.Api.Domain.Entities;

namespace Lectio.Api.Application.Seeding.LoanSeed;

public class LoanDataGenerator
{
    private const int LoansPerBook = 10;
    private const int DaysBetweenLoans = 15;
    private const int LoanPeriodDays = 15;

    public List<Loan> GenerateLoansForBook(int bookId, IReadOnlyList<int> userIds, DateTime startDate)
    {
        var usersToUse = Math.Min(LoansPerBook, userIds.Count);
        var loans = new List<Loan>(usersToUse);

        for (var i = 0; i < usersToUse; i++)
        {
            var checkoutDate = startDate.AddDays(DaysBetweenLoans * i);
            var dueDate = checkoutDate.AddDays(LoanPeriodDays);

            loans.Add(new Loan
            {
                BookId = bookId,
                UserId = userIds[i],
                CheckoutDate = checkoutDate,
                DueDate = dueDate,
                ReturnDate = dueDate
            });
        }

        return loans;
    }
}