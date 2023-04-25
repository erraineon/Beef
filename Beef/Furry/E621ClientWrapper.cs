using E621;

namespace Beef.Furry;

public class E621ClientWrapper : E621Client, IE621Client
{
    public E621ClientWrapper() : base("tadmor/errai")
    {
    }
}