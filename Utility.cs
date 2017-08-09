using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Collections.Specialized;

public static class ObjectCopier
{
    public static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}

[Serializable]
public class CRelatedCommandNoParam : ICommand
{
    readonly Func<Boolean> _canExecute;
    readonly Action _execute;
    public CRelatedCommandNoParam(Action execute) : this(execute, null)
    {
    }
    public CRelatedCommandNoParam(Action execute, Func<Boolean> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");
        _execute = execute;
        _canExecute = canExecute;
    }
    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute();
    }
    public event EventHandler CanExecuteChanged
    {
        add
        {

            if (_canExecute != null)
                CommandManager.RequerySuggested += value;
        }
        remove
        {

            if (_canExecute != null)
                CommandManager.RequerySuggested -= value;
        }
    }
    public void Execute(Object parameter)
    {
        _execute();
    }
}
[Serializable]
public class CRelatedCommandParam : ICommand
{
    readonly Func<Boolean> _canExecute;
    readonly Action<object> _execute;
    public CRelatedCommandParam(Action<object> execute) : this(execute, null)
    {
    }
    public CRelatedCommandParam(Action<object> execute, Func<Boolean> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");
        _execute = execute;
        _canExecute = canExecute;
    }
    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute();
    }
    public event EventHandler CanExecuteChanged
    {
        add
        {

            if (_canExecute != null)
                CommandManager.RequerySuggested += value;
        }
        remove
        {

            if (_canExecute != null)
                CommandManager.RequerySuggested -= value;
        }
    }
    public void Execute(Object parameter)
    {
        _execute(parameter);
    }
}

[Serializable]
public class AsyncObservableCollection<T> : ObservableCollection<T>
{
    [field: NonSerialized]
    private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    public AsyncObservableCollection()
    {
    }

    public AsyncObservableCollection(IEnumerable<T> list)
        : base(list)
    {
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (SynchronizationContext.Current == _synchronizationContext)
        {
            // Execute the CollectionChanged event on the current thread
            RaiseCollectionChanged(e);
        }
        else
        {
            // Post the CollectionChanged event on the creator thread
            _synchronizationContext.Post(RaiseCollectionChanged, e);
        }
    }

    private void RaiseCollectionChanged(object param)
    {
        // We are in the creator thread, call the base implementation directly
        base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_synchronizationContext == null) _synchronizationContext = SynchronizationContext.Current;

        if (SynchronizationContext.Current == _synchronizationContext)
        {
            // Execute the PropertyChanged event on the current thread
            RaisePropertyChanged(e);
        }
        else
        {
            // Post the PropertyChanged event on the creator thread
            _synchronizationContext.Post(RaisePropertyChanged, e);
        }
    }

    private void RaisePropertyChanged(object param)
    {
        // We are in the creator thread, call the base implementation directly
        base.OnPropertyChanged((PropertyChangedEventArgs)param);
    }
}