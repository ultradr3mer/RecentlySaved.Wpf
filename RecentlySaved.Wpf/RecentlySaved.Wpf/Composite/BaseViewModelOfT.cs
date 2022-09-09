using Mapster;

namespace RecentlySaved.Wpf.Composite
{
  public class BaseViewModel<T> : BaseViewModel
  {
    #region Fields

    private T attachedDataModel;

    #endregion Fields

    #region Methods

    public void SetDataModel(T data)
    {
      this.attachedDataModel = data;
      this.OnReadingDataModel(data);
    }

    public T WriteToDataModel()
    {
      return this.Adapt(this.attachedDataModel);
    }

    protected virtual void OnReadingDataModel(T data)
    {
    }




    #endregion Methods
  }
}
