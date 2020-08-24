using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TakeHome.Models;

namespace TakeHome.ViewModels
{
    public class OrderReviewViewModel : BaseViewModel
    {
        public ObservableCollection<OrderDetail> OrderLines { get; set; }
        public OrderReviewViewModel()
        {
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
        }

    }
}
