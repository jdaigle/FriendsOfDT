using System.Web;

namespace FODT.Security
{
    public interface ICookieCollection
    {
        void Add(HttpCookie cookie);
        HttpCookie Get(string name);
        void Clear(string name);
        void ClearAll();
        void RemoveAllFromResponse();
    }
}
