using System;

namespace ShopNest.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
	public decimal Amount { get; }

	public string Currency { get; }

	public static Money Zero(string currency = "USD")
	{
		return new Money(0m, currency);
	}

	public Money(decimal amount, string currency = "USD")
	{
		if (amount < 0m)
		{
			throw new ArgumentOutOfRangeException("amount", "Amount cannot be negative.");
		}
		if (string.IsNullOrWhiteSpace(currency))
		{
			throw new ArgumentException("Currency code is required.", "currency");
		}
		Amount = Math.Round(amount, 2);
		Currency = currency.ToUpperInvariant();
	}

	public Money Add(Money other)
	{
		EnsureSameCurrency(other);
		return new Money(Amount + other.Amount, Currency);
	}

	public Money Subtract(Money other)
	{
		EnsureSameCurrency(other);
		return new Money(Amount - other.Amount, Currency);
	}

	public Money Multiply(decimal factor)
	{
		return new Money(Amount * factor, Currency);
	}

	public Money ApplyPercentageDiscount(decimal percentage)
	{
		if ((percentage < 0m || percentage > 100m) ? true : false)
		{
			throw new ArgumentOutOfRangeException("percentage", "Percentage must be between 0 and 100.");
		}
		return new Money(Amount * (1m - percentage / 100m), Currency);
	}

	private void EnsureSameCurrency(Money other)
	{
		if (Currency != other.Currency)
		{
			throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} vs {other.Currency}.");
		}
	}

	public bool Equals(Money? other)
	{
		return other is not null && Amount == other.Amount && Currency == other.Currency;
	}

	public override bool Equals(object? obj)
	{
		return obj is Money other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Amount, Currency);
	}

	public override string ToString()
	{
		return $"{Amount:F2} {Currency}";
	}

	public static bool operator ==(Money left, Money right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Money left, Money right)
	{
		return !left.Equals(right);
	}

	public static bool operator >(Money left, Money right)
	{
		return left.Amount > right.Amount;
	}

	public static bool operator <(Money left, Money right)
	{
		return left.Amount < right.Amount;
	}

	public static bool operator >=(Money left, Money right)
	{
		return left.Amount >= right.Amount;
	}

	public static bool operator <=(Money left, Money right)
	{
		return left.Amount <= right.Amount;
	}
}
