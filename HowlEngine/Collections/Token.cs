namespace HowlEngine.Collections;

public struct Token{
    /// <summary>
    /// Gets the Id of the desired data within a Struct/ClassPool internal data structure.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// Gets the generation of the desired data within a Struct/ClassPool internal data structure.
    /// </summary>
    public readonly ushort Gen;

    /// <summary>
    /// Gets the validity of this token, whether or not the token will get the 'desired' data.
    /// </summary>
    public readonly bool Valid = false;
    
    /// <summary>
    /// Creates a new Token.
    /// </summary>
    /// <param name="id">The specified index of the desired data within a Struct/ClassPool internal data structure.</param>
    /// <param name="gen">The specified generation of the desired data within a Struct/ClassPool internal data structure.</param>
    public Token(int id, ushort gen){
        Id = id;
        Gen = gen;
        Valid = true;
    }

    public bool IsValid => Valid == true;
}