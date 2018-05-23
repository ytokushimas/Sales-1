using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Sales.Properties;

namespace Sales
{
    using ItemsType = ObservableCollection<Result>;
    class SalesMonthModel
    {
        private readonly SalesDatabaseContext _dataContext;
        private readonly SalesMonthModel _previousModel;
        public SalesMonthModel(
            SalesDatabaseContext dataContext, 
            byte month, 
            SalesMonthModel previousModel)
        {
            _resultItems.CollectionChanged += _resultItems_CollectionChanged;
            _dataContext = dataContext;
            _previousModel = previousModel;
            _month = month;
            Renew();
        }
        private readonly byte _month;
        public byte Month
        {
            get
            {
                return _month;
            }
        }
        public int PlanPrice
        {
            get
            {
                return Settings.Default.PlanPrice;
            }
        }
        public int ResultPrice { get; private set; }
        public int SubtractPrice
        {
            get
            {
                return ResultPrice - PlanPrice;
            }
        }
        public int TotalPrice
        {
            get
            {
                int p =
                _previousModel == null ?
                0 :
                _previousModel.TotalPrice;
                return p + SubtractPrice;
            }
        }
        private readonly ItemsType _resultItems = new ItemsType();
        public ItemsType ResultItems
        {
            get
            {
                return _resultItems;
            }
        }
        public void Renew()
        {
            //todo: 未実装(Sales.SalesMonthModel.Renew())
            //throw new System.NotImplementedException();
            var q =
                from p in _dataContext.Results
                where
                    p.Date.Year == Years.Item &&
                    p.Date.Month == Month &&
                    p.SectionId == Sections.Item.Id
                select p;

            _resultItems.Clear();
            foreach (Result x in q)
            {
                _resultItems.Add(x);
            }
        }
        public void Add(Result row)
        {
            _dataContext.Results.Add(row);
            _resultItems.Add(row);
        }
        public void Remove(Result row)
        {
            _dataContext.Results.Remove(row);
            _resultItems.Remove(row);
        }
        public int SaveChanges()
        {
            return _dataContext.SaveChanges();
        }

        private void _resultItems_CollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            var q =
            from p in _resultItems
            select p.Price;
            ResultPrice = q.Sum();
        }
    }
}
