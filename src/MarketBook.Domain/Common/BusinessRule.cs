namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Base class for business rules.
/// FA: کلاس پایه قوانین کسب‌وکار.
/// </summary>
public abstract class BusinessRule : IBusinessRule
{
    public abstract bool IsBroken();

    public abstract string Message { get; }

    /// <summary>
    /// EN: Throws when rule is broken.
    /// FA: در صورت نقض قانون استثناء ایجاد می‌کند.
    /// </summary>
    public void Check()
    {
        if (IsBroken())
        {
            throw new DomainException(
                new Error(
                    GetType().Name,
                    Message));
        }
    }
}