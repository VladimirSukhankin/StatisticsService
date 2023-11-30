using System;
using Xunit;
using FluentAssertions;

namespace TestProject;

public class BanksTests
{
    private readonly BankAccountBuilder _bankAccountBuilder = new();

    [Fact]
    public void Debit_WithValid_Amount_NotUpdatesBalance()
    {
        //Arrange
        var bankAccount = _bankAccountBuilder
            .WithName("Pol")
            .WithStartBalance(0)
            .WithAge(20)
            .Create();

        //Act
        var act = () => bankAccount.Debit(10);

        //Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("The amount is more than the balance");
    }

    [Fact]
    public void Age_Less_Than_18()
    {
        //Arrange
        var bankAccount = _bankAccountBuilder
            .WithName("Pols")
            .WithStartBalance(1)
            .WithAge(17)
            .Create();

        //Act
        var isCreatedAccount = bankAccount.IsCreateAccount();

        //Assert
        isCreatedAccount.Should().BeFalse();
    }
    
    [Fact]
    public void Debit_WithInvalidValid_Amount_NotUpdatesBalance()
    {
        //Arrange
        var bankAccount = _bankAccountBuilder
            .WithName("Poli")
            .WithStartBalance(100)
            .WithAge(20)
            .Create();

        //Act
        var act = () => bankAccount.Debit(-10);

        //Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("The amount must be greater than 0");
    }
    
    [Fact]
    public void Debit_WithValid_Amount_UpdatesBalance()
    {
        //Arrange
        var bankAccount = _bankAccountBuilder
            .WithName("Poli")
            .WithStartBalance(100)
            .WithAge(20)
            .Create();

        //Act
        bankAccount.Debit(10.123);

        //Assert
        bankAccount.Balance.Should().BePositive();
        bankAccount.Balance.Should().BeLessThan(100);
        bankAccount.Balance.Should().BeApproximately(90, 10.123);
        
            
    }
    
    [Fact]
    public void Credit_WithValid_Amount_UpdatesBalance()
    {
        //Arrange
        var bankAccount = _bankAccountBuilder
            .WithName("Poli")
            .WithStartBalance(100)
            .WithAge(20)
            .Create();

        //Act
        bankAccount.Credit(10.10);

        //Assert
        bankAccount.Balance.Should().BePositive();
        bankAccount.Balance.Should().BeGreaterThan(100);
        bankAccount.Balance.Should().BeApproximately(100, 10.10);
    }
}