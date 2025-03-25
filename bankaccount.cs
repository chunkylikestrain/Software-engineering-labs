namespace Bankaccount
{
    using System;

    public class BankAccount
    {
        public enum AccountStatus { Active, Closed, Inactive }

        protected decimal balance;
        protected bool isVerified;
        protected AccountStatus status;
        protected DateTime lastOperation;


        public BankAccount(bool verified)
        {
            this.isVerified = verified;
            balance = 0;
            status = AccountStatus.Active;
            lastOperation = DateTime.Now;
        }
        public void SetLastOperation(DateTime fakeLastOperation)
        {
            lastOperation = fakeLastOperation;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");

            balance += amount;
            UpdateStatusOnTransaction();
        }

        public void Withdraw(decimal amount)
        {
            if (status == AccountStatus.Closed)
                throw new InvalidOperationException("Cannot withdraw from a closed account.");

            if (!isVerified)
                throw new InvalidOperationException("Account not verified. Withdrawal not allowed.");

            if (amount > balance)
                throw new InvalidOperationException("Insufficient funds.");

            balance -= amount;
            UpdateStatusOnTransaction();
        }

        public void CloseAccount()
        {
            status = AccountStatus.Closed;
        }

        public void CheckForInactivity(TimeSpan inactivityPeriod, DateTime currentTime)
        {
            if (status == AccountStatus.Active && currentTime - lastOperation > inactivityPeriod)
            {
                status = AccountStatus.Inactive;
                lastOperation = currentTime; 
            }
        }

        public void ReactivateAccount()
        {
            if (status == AccountStatus.Inactive)
            {
                status = AccountStatus.Active;
                Console.WriteLine("Account has been reactivated");
            }
        }


        private void UpdateStatusOnTransaction()
        {
            Console.WriteLine($"Before transaction: Status={status}");

            if (status == AccountStatus.Inactive)
            {
                ReactivateAccount(); 
            }

            lastOperation = DateTime.Now;

            Console.WriteLine($"After transaction: Status={status}");
        }



        public decimal GetBalance() => balance;
        public AccountStatus GetStatus() => status;
    }
}


