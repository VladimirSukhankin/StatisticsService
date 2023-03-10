using ProjectForTest;

namespace TestProject;

public class BankAccountBuilder
{
    private readonly BankAccount _bankAccount = new();

    public BankAccountBuilder WithStartBalance(double balance)
    {
        _bankAccount.Balance = balance;
        return this;
    }

    public BankAccountBuilder WithAge(int age)
    {
        _bankAccount.AgeCustomer = age;
        return this;
    }
    
    public BankAccountBuilder WithName(string name)
    {
        return this;
    }

    public BankAccount Create()
    {
        return _bankAccount;
    }


}