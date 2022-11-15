using ProjectForTest;

namespace TestProject;

public class BankAccountBuilder
{
    private BankAccount _bankAccount = new BankAccount();

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
        _bankAccount.NameCustomer = name;
        return this;
    }

    public BankAccount Create()
    {
        return _bankAccount;
    }


}