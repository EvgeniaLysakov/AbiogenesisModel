using System.Globalization;
using System.Runtime.Serialization;
using AbiogenesisModel.Lib.Guard;

namespace AbiogenesisModel.Lib.Model;

/// <summary>
/// A Timestamp class provides a time stamp to objects. This time stamp could be used by other objects
/// to find how when an object was last changed for example.
/// </summary>
[DataContract]
public class Timestamp
{
    private static ulong _last;

    [DataMember]
    private ulong _timestamp;

    /// <summary>Gets an undefined timestamp.</summary>
    public static ulong Undefined => 0;

    /// <summary>
    /// Gets the next time stamp.
    /// </summary>
    private static ulong Next => Interlocked.Increment(ref _last);

    /// <summary>
    /// Gets the actual value of this timestamp
    /// </summary>
    public ulong Value => _timestamp;

    /// <summary>
    /// Sets the time stamp of the current object to be very old.
    /// </summary>
    public void Reset()
    {
        _timestamp = 0;
    }

    /// <summary>
    /// Sets the time stamp of the current object to be later than all current time stamps.
    /// </summary>
    public void Touch()
    {
        _timestamp = Next;
    }

    /// <summary>
    /// Returns true if the time stamp has been touched (and has not been <see cref="Reset"/>)
    /// </summary>
    public bool Touched => _timestamp != 0;

    /// <summary>
    /// Sets the time stamp of the current object to be no older than that of the source object.
    /// </summary>
    /// <param name="source">the source object whose time stamp should be taken</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the specified <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public Timestamp SetAtLeast(Timestamp source)
    {
        Ensure.That(source).IsNotNull();

        if (_timestamp < source._timestamp)
        {
            _timestamp = source._timestamp;
        }

        return this;
    }

    /// <summary>
    /// Checks whether the current object has a time stamp later than another object.
    /// </summary>
    /// <param name="who">the time stamp to compare with</param>
    /// <returns>
    /// boolean indicating if this object is more recent than the other object
    /// </returns>
    public bool IsLaterThan(Timestamp who)
    {
        return _timestamp > who._timestamp;
    }

    /// <summary>
    /// Convert this time stamp to a string.
    /// </summary>
    /// <returns>string representation of this time stamp object</returns>
    public override string ToString()
    {
        return _timestamp.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Compares the time stamps of two objects.
    /// </summary>
    /// <param name="ts1">the first time stamp to compare</param>
    /// <param name="ts2">the second time stamp to compare</param>
    /// <returns>
    /// boolean indicating if this first object is less recent than the second object
    /// </returns>
    public static bool operator <(Timestamp ts1, Timestamp? ts2)
    {
        return ts2 != null && ts2.IsLaterThan(ts1);
    }

    /// <summary>
    /// Compares the time stamps of two objects.
    /// </summary>
    /// <param name="ts1">the first time stamp to compare</param>
    /// <param name="ts2">the second time stamp to compare</param>
    /// <returns>
    /// boolean indicating if this first object is more recent than the second object
    /// </returns>
    public static bool operator >(Timestamp? ts1, Timestamp ts2)
    {
        return ts1 != null && ts1.IsLaterThan(ts2);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Timestamp other)
        {
            return _timestamp == other._timestamp;
        }

        return false;
    }

    protected bool Equals(Timestamp other)
    {
        return _timestamp == other._timestamp;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return _timestamp.GetHashCode();
    }
}