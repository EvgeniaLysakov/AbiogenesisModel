using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbiogenesisModel.App;

public sealed class EventCounterViewModel : INotifyPropertyChanged
{
    private long _attempts;
    private long _successes;
    private long _noEffect;

    public event PropertyChangedEventHandler? PropertyChanged;

    public EventCounterViewModel(string eventName, long attempts, long successes, long noEffect)
    {
        EventName = eventName;
        _attempts = attempts;
        _successes = successes;
        _noEffect = noEffect;
    }

    public string EventName { get; }

    public long Attempts
    {
        get => _attempts;
        set => SetField(ref _attempts, value);
    }

    public long Successes
    {
        get => _successes;
        set => SetField(ref _successes, value);
    }

    public long NoEffect
    {
        get => _noEffect;
        set => SetField(ref _noEffect, value);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}