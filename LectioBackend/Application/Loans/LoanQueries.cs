using System.Linq.Expressions;
using Lectio.Api.Domain.Entities;

namespace Lectio.Api.Application.Loans;

public static class LoanQueries
{
    public static Expression<Func<Loan, bool>> IsActiveForBook(int bookId) =>
        loan => loan.BookId == bookId && loan.ReturnDate == null;
}
