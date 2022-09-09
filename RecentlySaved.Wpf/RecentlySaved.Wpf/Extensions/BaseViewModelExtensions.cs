using Mapster;
using RecentlySaved.Wpf.Composite;

namespace RecentlySaved.Wpf.Extensions
{
  public static class BaseViewModelExtensions
  {
    public static Tvm GetWithDataModel<Tvm, Tdata>(this Tvm value, Tdata data) where Tvm : BaseViewModel<Tdata>
    {
      value.SetDataModel(data);
      data.Adapt<Tdata,Tvm>(value);
      return value;
    }
  }
}
