using Microsoft.CSharp.RuntimeBinder;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            string cola = "1";
            VendingMachine vendingMachine = new VendingMachine();
            vendingMachine.addCollectedCash(1);
            vendingMachine.setCollectedCash(1);
            vendingMachine.getState();
            vendingMachine.dispenseItem(cola);
        }
    }
    public interface IState
    {
        void CollectCash(int cash);
        void DispenseChange(String productCode);
        void DispenseItem(String productCode);
        void CancelTransaction();
    }
    public class VendingMachine
    {
        private int collectedCash;
        private IState state;
        private Dictionary<string, string> productCodeItemMap;
        private Dictionary<string, int> productCodePriceMap;

        public void addCollectedCash(int collectedCash)
        {
            this.collectedCash += collectedCash;
        }

        public VendingMachine setCollectedCash(int collectedCash)
        {
            this.collectedCash = collectedCash;
            return this;
        }

        public IState getState()
        {
            return state;
        }

        public VendingMachine setState(IState state)
        {
            this.state = state;
            return this;
        }

        public void dispenseChange(string productCode)
        {
            this.state.DispenseChange(productCode);
        }

        public void cancelTransaction()
        {
            this.state.CancelTransaction();
        }

        public void dispenseItem(string productCode)
        {
            this.state.DispenseItem(productCode);
        }

        public int getCollectedCash()
        {
            return collectedCash;
        }

    }

    public class DispenseChange : IState
    {
        private VendingMachine vendingMachine;

        public DispenseChange(VendingMachine vendingMachine)
        {
            this.vendingMachine = vendingMachine;
        }
        public void CancelTransaction()
        {
            throw new RuntimeBinderException("Unable to cancel transaction");
        }

        public void CollectCash(int cash)
        {
            throw new RuntimeBinderException("Unable Dispense Change. Unable to collect cash");
        }

        public void DispenseItem(string productCode)
        {
            throw new RuntimeBinderException("Unable to dispense Item");
        }

        void IState.DispenseChange(string productCode)
        {
            this.vendingMachine.setState(new DispenseItem(this.vendingMachine));
            this.vendingMachine.dispenseItem(productCode);
        }
    }

    public class DispenseItem : IState
    {
        private VendingMachine vendingMachine;

        public DispenseItem(VendingMachine vendingMachine)
        {
            this.vendingMachine = vendingMachine;
        }
        public void CancelTransaction()
        {
            this.vendingMachine.setState(new TransactionCancelled(vendingMachine));
            this.vendingMachine.cancelTransaction();
        }

        public void CollectCash(int cash)
        {
            this.vendingMachine.addCollectedCash(cash);
        }

        public void DispenseChange(string productCode)
        {
            throw new NotImplementedException();
        }

        void IState.DispenseItem(string productCode)
        {
            Console.WriteLine("Dispensing Item:" + productCode);
            vendingMachine.setState(new Ready(this.vendingMachine));
        }
    }

    public class Ready : IState
    {
        private VendingMachine vendingMachine;

        public Ready(VendingMachine vendingMachine)
        {
            this.vendingMachine = vendingMachine;
        }
        public void CancelTransaction()
        {
            this.vendingMachine.setState(new TransactionCancelled(vendingMachine));
            this.vendingMachine.cancelTransaction();
        }

        public void CollectCash(int cash)
        {
            this.vendingMachine.addCollectedCash(cash);
        }

        public void DispenseChange(string productCode)
        {
            this.vendingMachine.setState(new DispenseChange(this.vendingMachine));
            this.vendingMachine.dispenseChange(productCode);
        }

        public void DispenseItem(string productCode)
        {
            throw new RuntimeBinderException("Unable to dispense Item");
        }
    }

    public class TransactionCancelled : IState
    {
        private VendingMachine vendingMachine;

        public TransactionCancelled(VendingMachine vendingMachine)
        {
            this.vendingMachine = vendingMachine;
        }
        public void CancelTransaction()
        {
            Console.WriteLine("Returing collected cash" + vendingMachine.getCollectedCash());
            vendingMachine.setCollectedCash(0);
            vendingMachine.setState(new Ready(vendingMachine));
        }

        public void CollectCash(int cash)
        {
            throw new NotImplementedException();
        }

        public void DispenseChange(string productCode)
        {
            throw new NotImplementedException();
        }

        public void DispenseItem(string productCode)
        {
            throw new NotImplementedException();
        }
    }

}
