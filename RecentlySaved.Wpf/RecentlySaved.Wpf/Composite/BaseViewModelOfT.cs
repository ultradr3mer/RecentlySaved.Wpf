using Mapster;

namespace RecentlySaved.Wpf.Composite
{
  public class BaseViewModel<T> : BaseViewModel where T : class
  {
    #region Fields

    private T attachedDataModel;
    protected bool IsReadingDataModel { get; private set; }

    #endregion Fields

    #region Methods

    public void SetDataModel(T value)
    {
      this.IsReadingDataModel = true;
      this.attachedDataModel = value;
      value.Adapt(this, value.GetType(), this.GetType());
      this.OnReadingDataModel(value);
      this.IsReadingDataModel = false;
    }

    internal bool DatamodelEquals(T data)
    {
      return this.attachedDataModel == data;
    }

    protected virtual void OnReadingDataModel(T data)
    {

    }

    public T WriteToDataModel()
    {
      return (T)this.Adapt(this.attachedDataModel, this.GetType(), this.attachedDataModel.GetType());
    }

    #endregion Methods
  }
}
