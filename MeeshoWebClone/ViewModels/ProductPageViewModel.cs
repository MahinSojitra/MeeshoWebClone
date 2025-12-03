using MeeshoWebClone.Models;

namespace MeeshoWebClone.ViewModels
{
    public class ProductPageViewModel
    {
        public List<ProductCategory> Categories { get; set; }
        public List<ProductDisplayViewModel> Products { get; set; }
    }

}
