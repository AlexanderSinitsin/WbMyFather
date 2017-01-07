using System.Web;

namespace WebSite
{
    public class SessionManager
    {
        private readonly HttpSessionStateBase m_session;

        public SessionManager(HttpSessionStateBase session)
        {
            m_session = session;
        }

        public T Get<T>(string key, T defaultValue = null) where T : class
        {
            return getObject(key) as T ?? defaultValue;
        }

        public int Get(string key, int defaultValue)
        {
            int value;
            return int.TryParse(Get(key), out value) ? value : defaultValue;
        }

        public bool Get(string key, bool defaultValue)
        {
            bool value;
            return bool.TryParse(Get(key), out value) ? value : defaultValue;
        }

        public string Get(string key, string defaultValue = null)
        {
            var value = getObject(key);
            return value?.ToString() ?? defaultValue;
        }

        public void Set(string key, object value)
        {
            m_session[key] = value;
        }

        private object getObject(string key)
        {
            return m_session[key];
        }
    }
}
