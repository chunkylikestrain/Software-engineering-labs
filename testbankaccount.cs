namespace testBankAccount
{
    using NUnit.Framework;
    using System;
    using Bankaccount;
    public class BankAccountTests
    {
        private BankAccount account; 

        [SetUp]
        public void Setup()
        {
            account = new BankAccount(true); 
        }

        [Test]
        public void Deposit_ShouldIncreaseBalance()
        {
            account.Deposit(100);
            Assert.That(account.GetBalance(), Is.EqualTo(100));
        }

        [Test]
        public void Deposit_NegativeAmount_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => account.Deposit(-100));
        }

        [Test]
        public void Withdraw_ValidAmount_ShouldDecreaseBalance()
        {
            account.Deposit(200);
            account.Withdraw(100);
            Assert.That(account.GetBalance(), Is.EqualTo(100)); // assuming GetBalance() returns the current balance
        }

        [Test]
        public void Withdraw_WithoutVerification_ShouldThrowException()
        {
            var unverifiedAccount = new BankAccount(false);
            Assert.Throws<InvalidOperationException>(() => unverifiedAccount.Withdraw(50));
        }

        [Test]
        public void Withdraw_MoreThanBalance_ShouldThrowException()
        {
            account.Deposit(50);
            Assert.Throws<InvalidOperationException>(() => account.Withdraw(100));
        }

        [Test]
        public void CloseAccount_ShouldPreventWithdrawals()
        {
            account.CloseAccount();
            Assert.Throws<InvalidOperationException>(() => account.Withdraw(10));
        }

        [Test]
        public void Inactivity_ShouldDeactivateAccount()
        {
            DateTime fakeNow = DateTime.Now;
            account.SetLastOperation(fakeNow - TimeSpan.FromDays(31)); 
            account.CheckForInactivity(TimeSpan.FromDays(30), fakeNow);

            Assert.That(account.GetStatus(), Is.EqualTo(BankAccount.AccountStatus.Inactive)); 
        }

        [Test]
        public void Reactivation_ShouldActivateAccountOnNewTransaction()
        {
            DateTime fakeNow = DateTime.Now;
            account.SetLastOperation(fakeNow - TimeSpan.FromDays(31)); 
            account.CheckForInactivity(TimeSpan.FromDays(30), fakeNow);
            Assert.That(account.GetStatus(), Is.EqualTo(BankAccount.AccountStatus.Inactive));

            using (var consoleOutput = new ConsoleOutput())
            {
                account.Deposit(50); 
                string output = consoleOutput.GetOutput();
                Console.WriteLine($"Captured Output: {output}"); 

                Assert.That(account.GetStatus(), Is.EqualTo(BankAccount.AccountStatus.Active));
                Assert.That(output.Contains("Account has been reactivated"), Is.True);
            }
        }

        [Test]
        public void Reactivation_ShouldTriggerMessage()
        {
            DateTime fakeNow = DateTime.Now;
            account.SetLastOperation(fakeNow - TimeSpan.FromDays(31)); 
            account.CheckForInactivity(TimeSpan.FromDays(30), fakeNow);
            Assert.That(account.GetStatus(), Is.EqualTo(BankAccount.AccountStatus.Inactive));

            using (var consoleOutput = new ConsoleOutput())
            {
                account.Deposit(50);
                string output = consoleOutput.GetOutput();
                Console.WriteLine($"Captured Output: {output}"); // Debugging log

                Assert.That(output.Contains("Account has been reactivated"), Is.True);
            }
        }

    }

    // Helper class to capture console output
    public class ConsoleOutput : IDisposable
    {
        private System.IO.StringWriter stringWriter;
        private System.IO.TextWriter originalOutput;

        public ConsoleOutput()
        {
            stringWriter = new System.IO.StringWriter();
            originalOutput = Console.Out;
            Console.SetOut(stringWriter);
        }

        public string GetOutput()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            stringWriter.Dispose();
        }
    }
}
