namespace HowlEngine.ECS;

public struct Token{
    public int Id;
    public ushort Gen;
    public bool Valid = false;
    
    public Token(int id, ushort gen){
        Id = id;
        Gen = gen;
        Valid = true;
    }
}