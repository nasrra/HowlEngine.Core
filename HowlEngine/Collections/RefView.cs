namespace HowlEngine.Collections;

/// <summary>
/// A wrapper class to ensure that returned references are valid.
/// </summary>
/// <typeparam name="T">The type of data being referenced.</typeparam>
public ref struct RefView<T>{
    /// <summary>
    /// Gets the data that is being referenced.
    /// </summary>
    public readonly ref T Data;

    /// <summary>
    /// Gets whether or not the reference is valid in terms of being the the 'desired' data.
    /// </summary>
    public readonly bool Valid = false;

    /// <summary>
    /// Creates a new RefView.
    /// </summary>
    /// <param name="data">The data to pass for referencing.</param>
    /// <param name="valid">Whether or not the reference is valid.</param>
    public RefView(ref T data, bool valid){
        Data = ref data;
        Valid = valid;
    }
}