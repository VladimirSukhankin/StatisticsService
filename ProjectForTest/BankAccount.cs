using System.Data;

namespace ProjectForTest;

public class BankAccount
{
    public int AgeCustomer { get; set; }
    public string NameCustomer { get; set; }
    public BankAccount()
    {
        
    }
    public BankAccount(double balance)
    {
        Balance = balance;
    }

    public double Balance { get; set; }

    public void Debit(double amount)
    {
        if (amount > Balance)
        {
            throw new ArgumentOutOfRangeException("The amount is more than the balance");
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException("The amount must be greater than 0");
        }

        Balance -= amount;
    }

    public void Credit(double amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        Balance += amount;
    }

    public bool IsCreateAccount()
    {
        return AgeCustomer > 18;
    }
    
}